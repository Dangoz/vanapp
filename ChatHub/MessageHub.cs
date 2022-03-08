using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Models;
using Server.Types;
using Server.Data;
using Microsoft.AspNetCore.SignalR;
using Server.Extensions;

namespace Server.ChatHub
{
    public class MessageHub : Hub
    {
        private readonly MessageRepository _messageRepository;
        private readonly UserRepository _userRepository;
        public MessageHub(MessageRepository messageRepository,UserRepository userRepository)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"].ToString();

            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = await AddToGroup(groupName);

            var messages = await _messageRepository.
                GetMessageThread(Context.User.GetUsername(), otherUser);

            await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group = await RemoveFromMessageGroup();
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(MessageType message_obj) {
            Console.WriteLine("222222222222222");
            var username = Context.User.GetUsername();

            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var recipient = await _userRepository.GetUserByUsernameAsync(message_obj.RecipientUsername);

            if (recipient == null) throw new HubException("Not found user");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = message_obj.Message
            };

            var groupName = GetGroupName(sender.UserName, recipient.UserName);

            var group = await _messageRepository.GetMessageGroup(groupName);

            var savedMsg = _messageRepository.AddMessage(message);
            var saveResult = await _messageRepository.SaveAllAsync();
            Console.WriteLine("33333333333333");
            Console.WriteLine(saveResult);
 
                Console.WriteLine("444444444444444444");
                Console.WriteLine(sender.UserName);
                Console.WriteLine(recipient.UserName);
                Console.WriteLine(savedMsg.MessageSent);

                var msgResponse = new
                {
                    senderUsername = sender.UserName,
                    messageSent = savedMsg.MessageSent,
                    messageContent = message_obj.Message
                };
                await Clients.Group(groupName).SendAsync("NewMessage", msgResponse);
            
        }

        private async Task<Group> AddToGroup(string groupName)
        {
            var group = await _messageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

            if (group == null)
            {
                group = new Group(groupName);
                _messageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);

            if (await _messageRepository.SaveAllAsync()) return group;

            throw new HubException("Failed to join group");
        }

        private async Task<Group> RemoveFromMessageGroup()
        {
            var group = await _messageRepository.GetGroupForConnection(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            _messageRepository.RemoveConnection(connection);
            if (await _messageRepository.SaveAllAsync()) return group;

            throw new HubException("Failed to remove from group");
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}