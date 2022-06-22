const key = 'userData';

const Auth = {
  isAuthenticated() {
    const userData = this.getUserData();
    return userData && userData.token !== '';
  },
  authenticate(user, token) {
    const userData = JSON.stringify({ user, token });
    localStorage.setItem(key, userData);
  },
  signOut() {
    localStorage.removeItem(key);
  },
  getUserData() {
    const data = localStorage.getItem(key);
    if (data) {
      const userData = JSON.parse(data);
      return userData;
    }
    return null;
  },
};

export default Auth;
