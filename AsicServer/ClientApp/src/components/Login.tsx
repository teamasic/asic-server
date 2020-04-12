import * as React from 'react';
import { connect } from 'react-redux';
import { Form, Icon, Input, Button, Checkbox, Spin, Row, Col } from 'antd';
import { FormEvent } from 'react';
import { FormComponentProps } from 'antd/lib/form';

import '../styles/LoginForm.css';
import { ApplicationState } from '../store';
import { bindActionCreators } from 'redux';
import { userActionCreators } from '../store/user/userActionCreators';
import UserLogin from '../models/UserLogin';
import { UserState } from '../store/user/userState';
import { RouteComponentProps } from 'react-router';
import * as firebase from '../firebase';
import Text from 'antd/lib/typography/Text';
import { getErrors, error } from '../utils';

const redirectLocation = '/dashboard';
// At runtime, Redux will merge together...
type LoginProps =
  UserState // ... state we've requested from the Redux store
  & typeof userActionCreators // ... plus action creators we've requested
  & FormComponentProps
  & RouteComponentProps<{}>; // ... plus incoming routing parameters

interface LoginComponentState {
  errorMessage: string
}

class NormalLoginForm extends React.Component<LoginProps, LoginComponentState> {

  state = {
    errorMessage: ""
  }

  constructor(props: LoginProps) {
    super(props);

  }

  public componentDidMount() {
    firebase.auth.onAuthStateChanged(authUser => {
      if (authUser) {
        authUser.getIdToken().then(token => {
          console.log(token);
          const credentials = { firebaseToken: token };
          this.login(credentials);
        });
      }
    });
  }

  handleSubmit = (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    this.props.form.validateFields((err, values) => {
      if (!err) {
        console.log('credentials: ', values);
        this.login({ username: values.username, password: values.password });
      }
    });
  };

  redirect = () => {
    window.location.replace(redirectLocation);
  }

  login = (userCredentials: UserLogin) => {
    this.props.requestLogin(userCredentials, this.redirect);
  }

  render() {
    return (
      <div className="container">
        <Row align="middle">
          <div className="content">
            <Col>{(this.props.isLoading) ? <Spin /> : this.getForm()}</Col>
          </div>
        </Row>
      </div>
    );
  }

  getForm() {
    const { getFieldDecorator } = this.props.form;
    return (
      <Form onSubmit={this.handleSubmit} className="login-form">
        <Form.Item>
          {getFieldDecorator('username', {
            rules: [{ required: true, message: 'Please input your username!' }],
          })(
            <Input
              prefix={<Icon type="user" style={{ color: 'rgba(0,0,0,.25)' }} />}
              placeholder="Username"
            />,
          )}
        </Form.Item>
        <Form.Item>
          {getFieldDecorator('password', {
            rules: [{ required: true, message: 'Please input your Password!' }],
          })(
            <Input
              prefix={<Icon type="lock" style={{ color: 'rgba(0,0,0,.25)' }} />}
              type="password"
              placeholder="Password"
            />,
          )}
        </Form.Item>
        <Form.Item>
          <Button type="primary" htmlType="submit" className="login-form-button">
            Log in
          </Button>
        </Form.Item>
        <Form.Item>
          <Button type='primary' onClick={firebase.auth.doSignInWithGooogle}>Sign in with Google</Button>
          {
            this.props.errors.length === 0 ? "" :
              this.renderErrors()
          }
        </Form.Item>
      </Form>);
  }

  private renderErrors() {
    firebase.auth.doSignOut().then(() => {
      error(getErrors(this.props.errors))
    });
  }
}


const WrappedNormalLoginForm = Form.create({ name: 'normal_login' })(NormalLoginForm);

const matchDispatchToProps = (dispatch: any) => {
  return bindActionCreators(userActionCreators, dispatch);
}
export default connect((state: ApplicationState) => state.user, matchDispatchToProps)(WrappedNormalLoginForm);
