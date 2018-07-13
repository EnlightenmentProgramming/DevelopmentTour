/**
 * @author [MrLong]
 * @description [登录组件状态管理]
 */
import { sendProcess , deepCopy } from '../../common/commonHelper'
import md5 from 'js-md5'

export default {
	namespace: true,
	state: {
		G_name: null,//登录框输入的账号
		G_pass: null,//登录框中输入的密码
		G_hint: null,//登录提示信息
		G_submit: null,//登录按钮状态
	},//state The End
	mutations: {
		upName(state,name){
			state.G_name = name
		},
		upPass(state,pass){
			state.G_pass = pass
		},
		upHint(state,hint){
			state.G_hint = hint
		},
		upSubmit(state,submt){
			state.G_submit = submt
		},
	},//mutations The End 
	actions: {
		/**
		 * [login 登录方法]
		 * @param  {[type]} options.state [description]
		 * @return {[type]}               [description]
		 */
		login({ dispatch, state, rootState}){
			//取全局中的报文头
			//console.log(rootState);
			var hMsg = deepCopy(rootState.headMsg);
			//给报文头的接口名称赋值
			hMsg.Method = 'Login';
			var reqMsg = deepCopy(rootState.requestMsg);
			var requestParas = {
				A_UserID: state.G_name,
				A_Pwd: md5(state.G_pass),
			}
			reqMsg.RequestParams = JSON.stringify(requestParas);			
			dispatch('sendSocketMsg',sendProcess(hMsg,reqMsg));
		},
	},//actions The End
}