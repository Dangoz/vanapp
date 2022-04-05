import axios from "axios";
import { API_URL } from "../config/constants";

export const checkLocationApi = async (cb) => {
  // axios
  //   .get("https://api64.ipify.org/?format=json", {
  //     withCredentials: true,
  //   })
  //   .then((response) => {
  //     console.log("vvvvvvvvvvvvvvvvvvv");
  //     console.log(response);
  //     axios
  //       .post(`${API_URL}/accountapi/ip`, response, {
  //         withCredentials: true,
  //       })
  //       .then((response) => {
  //         cb(null, response);
  //       })
  //       .catch((error) => {
  //         console.log(error);
  //       });
  //   })
  //   .catch((error) => {
  //     console.log(error);
  //   });

  axios
    .get(`${API_URL}/accountapi/ip`, {
      withCredentials: true,
    })
    .then((response) => {
      cb(null, response);
    })
    .catch((error) => {
      console.log(error);
    });
};

export const loginApi = (user_obj, cb) => {
  axios
    .post(`${API_URL}/accountapi/login`, user_obj)
    .then((response) => {
      cb(null, response);
    })
    .catch((error) => {
      window.alert("Please check your credential, or that you are registered.");
      console.log(error);
    });
};

export const registerApi = (user_obj, cb) => {
  console.log(`${API_URL}/accountapi/register`);
  console.log(user_obj);

  axios
    .post(`${API_URL}/accountapi/register`, user_obj)
    .then((response) => {
      cb(null, response);
    })
    .catch((error) => {
      window.alert(
        "Please make sure that passwords contain an uppercase character, lowercase character, a digit, and a non-alphanumeric character. Passwords must be at least six characters long."
      );
      console.log(error);
    });
};

export const getUsersApi = (cb) => {
  axios
    .get(`${API_URL}/UsersApi/users`, {
      withCredentials: true,
    })
    .then((response) => {
      cb(null, response);
    })
    .catch((error) => {
      console.log(error);
    });
};

export const uploadImg = (form_obj, cb) => {
  axios
    .post(`${API_URL}/UsersApi/img`, form_obj.formData, {
      headers: { Authorization: `Bearer ${form_obj.cuser.jwt}` },
    })
    .then((response) => {
      cb(null, response);
    })
    .catch((error) => {
      console.log(error);
    });
};
