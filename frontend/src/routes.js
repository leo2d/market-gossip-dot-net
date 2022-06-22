import React from 'react';
import { Route, Switch, Redirect } from 'react-router-dom';
import Chat from './pages/Chat';
import Login from './pages/Login';
import Auth from './utils/auth';

const PrivateRoute = ({ component: Component, ...rest }) => (
  <Route
    {...rest}
    render={props =>
      Auth.isAuthenticated() ? (
        <Component {...props} />
      ) : (
        <Redirect
          to={{
            pathname: '/',
          }}
        />
      )
    }
  />
);

const Routes = () => {
  return (
    <Switch>
      <Route path="/" exact component={Login} />
      <Route path="/login" exact component={Login} />
      <PrivateRoute path="/chat" component={Chat} />
    </Switch>
  );
};

export default Routes;
