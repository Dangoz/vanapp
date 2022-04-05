const baseUrl = process.env.NODE_ENV === 'production' ? "https://vanapp.azurewebsites.net" : "http://localhost:5198";
const API_URL = `${baseUrl}/api`;

const HUB_URL = `${baseUrl}/hubs`;

export { API_URL, HUB_URL };
