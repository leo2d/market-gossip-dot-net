import axios from 'axios';
import serverURL from '../serverUrl';

const Api = axios.create({
  baseURL: `${serverURL}/api`,
});

export default Api;
