import * as React from 'react';
import { Container } from 'reactstrap';
import NavMenu from './NavMenu';
import { Layout, Menu, Breadcrumb, Icon, Divider, Row, Col } from 'antd';
import '../styles/Layout.css';
import classNames from 'classnames';
import { constants } from '../constants/constant';
import * as firebase from '../firebase';
import { UserState } from '../store/user/userState';
import { userActionCreators } from '../store/user/userActionCreators';
import { ApplicationState } from '../store';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { Link, withRouter } from 'react-router-dom';
import MenuBar from './MenuBar';

const { Header, Sider, Content, Footer } = Layout;
type LayoutProps =
	UserState &
	typeof userActionCreators; // ... plus incoming routing parameters

class PageLayout extends React.Component<
	LayoutProps,
	{
		collapsed: boolean;
	}
	> {
	state = {
		collapsed: false
	};

	constructor(props: LayoutProps) {
		super(props);
	}

	onCollapse = (collapsed: boolean) => {
		this.setState({ collapsed });
	};

	render() {
		return (<>{this.props.isLogin && this.props.successfullyLoaded ? this.renderLayout() : this.renderEmty()}</>);
	}
	private renderLayout() {
		return (
			<Layout className="layout">
				<Sider
					className="sider"
					collapsible
					collapsed={this.state.collapsed}
					onCollapse={this.onCollapse}
				>
					<div className="logo" >
						<img
							src="asiclogo.jpg"
							style={{
								backgroundSize: 'contain',
								backgroundPosition: 'center'
							}} />
					</div>
					<Menu theme="dark" defaultSelectedKeys={['1']} mode="inline">
						<Menu.Item key="1">
							<Icon type="user" />
							<div className="link-container">
								<Link to="/dashboard">
									Manage users
								</Link>
							</div>
						</Menu.Item>
						<Menu.Item key="train">
							<Icon type="sync" />
							<div className="link-container">
								<Link to="/train-model">
									Train model
								</Link>
							</div>
						</Menu.Item>
						<Menu.Item key="3" onClick={(e) => this.logout()}>
							<Icon type="logout" />
							<span>Logout</span>
						</Menu.Item>
					</Menu>
				</Sider>
				<Layout className={classNames({
					'inner-layout': true,
					'with-sidebar-collapsed': this.state.collapsed
				})}>
					<MenuBar />
					<Content className="content">
						{this.props.children}
					</Content>
				</Layout>
			</Layout >

		);
	}
	private renderEmty() {
		return (
			<Layout className="layout">
				<Row type='flex' align='middle' justify='space-around' >
					<Col span={8} >
						{this.props.children}
					</Col>
				</Row>
			</Layout>
		);
	}

	private logout() {
		if (this.props.isLogin) {
			firebase.auth.doSignOut().then(() => {
				localStorage.removeItem(constants.AUTH_IN_LOCAL_STORAGE);
				localStorage.removeItem(constants.ACCESS_TOKEN);
				this.props.logout();
			});
		}
	}
}

export default withRouter(connect(
	(state: ApplicationState) => ({
		...state.user
	}), // Selects which state properties are merged into the component's props
	dispatch => bindActionCreators({
		...userActionCreators
	}, dispatch) // Selects which action creators are merged into the component's props
)(PageLayout as any));
