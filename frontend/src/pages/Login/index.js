import React, { useState, useEffect } from 'react';
import { useHistory } from 'react-router-dom';
import { Container } from 'react-bootstrap';
import { AuthContainer } from './styles';
import Api from '../../services/api';
import Auth from '../../utils/auth';
import AuthForm from '../../components/AuthForm';

const Login = () => {
  const history = useHistory();

  const [userData, setUserData] = useState({
    email: '',
    password: '',
    username: '',
  });

  const [isLogin, setIsLogin] = useState(true);

  useEffect(() => {
    window.document.title = isLogin
      ? 'Log In - Market Gossip'
      : 'Sign Up - Market Gossip';
  });

  const onResponseSuccess = response => {
    const token = response.data?.token;
    console.log('response -> ', response);
    const { username, email } = userData;

    const colors = ['#7BC0A3', '#E8E9A1', '#FD7E89', '#E6B566', '#FFFFC2', '#CBAF87', '#FABEA7', '#FFCFDF'];
    const userColor = colors[Math.floor(Math.random() * colors.length)];


    if (token && username) {
      Auth.authenticate({ username, email, userColor }, token);
      history.push('/chat');
    }
  };

  const loginSubmit = async _ => {
    const { username, password } = userData;
    const response = await Api.post('/authentication/signin', {
      username,
      password,
    }).catch(error => {
      if (error?.response?.status === 401) {
        window.alert('Invalid login credentials. Please try again.');
        return;
      } else return error;
    });

    if (response && response.status === 200) {
      onResponseSuccess(response);
    }
  };

  const signUpSubmit = async _ => {
    const response = await Api.post('/authentication/signup', userData).catch(
      error => {
        if (error?.response?.status === 400) {
          const errors = error.response.data.errors;

          for (let i = 0; i < errors.length; i++) window.alert(`${errors[i]}`);

          return;
        }
        return error;
      }
    );

    if (response && response.status === 200) {
      await loginSubmit();
    }
  };

  const handleSubmit = async event => {
    event.preventDefault();
    if (isLogin) await loginSubmit();
    else await signUpSubmit();
  };

  return (
    <Container>
      <AuthContainer>
        <AuthForm
          handleSubmit={handleSubmit}
          userData={userData}
          setUserData={setUserData}
          isLogin={isLogin}
          setIsLogin={setIsLogin}
        />
      </AuthContainer>
    </Container>
  );
};

export default Login;
