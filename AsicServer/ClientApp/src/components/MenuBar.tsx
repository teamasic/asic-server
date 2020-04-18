import * as React from 'react';
import { ApplicationState } from '../store';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import NavMenu from './NavMenu';
import '../styles/MenuBar.css';
import { Layout, Menu, Breadcrumb, Icon, Dropdown, Badge, Row, Col, Spin, Avatar, Button } from 'antd';
import { Link, withRouter } from 'react-router-dom';
import { RouteComponentProps } from 'react-router';
import * as firebase from '../firebase';
import { UserState } from '../store/user/userState';
import { userActionCreators } from '../store/user/userActionCreators';
const { Header, Sider, Content, Footer } = Layout;

// At runtime, Redux will merge together...
type Props =
	UserState &
	typeof userActionCreators &
	RouteComponentProps<{}>; // ... plus incoming routing parameters


class MenuBar extends React.Component<Props> {
	render() {
		return <Content className="menu-bar row">
				<Avatar className="avatar" src={this.props.currentUser.image} />
				<div className="fullname">{this.props.currentUser.name}</div>
			</Content>;
	}

}

export default withRouter(connect(
	(state: ApplicationState) => ({
		...state.user,
	})
)(MenuBar as any));