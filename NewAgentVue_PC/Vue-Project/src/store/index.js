/**
 * @author [MrLong]
 * @date [2018-07-01]
 * @description [组装模块并导出store]
 */
import Vue from 'vue'
import Vuex from 'vuex'
/**
 * @description [导入需要的store]
 */
import websocket from './WebSocket'
import loginStore from './userInfo/loginStore'

Vue.use(Vuex)

export default new Vuex.Store({
	state: {
		isLogin : '0',//标记是否登录成功
		headMsg: {
			Account: null,
			LoginID: null,
			Token: null,
			Ip: null,
			Method: null,
			ExeMode: null,
		},//报文包头对象
		requestMsg: {
			IsZ: "Y",//应答报文是否压缩
			RType: "1",//应答报文数据格式
			CFlag: "4",//客户端类型
			CVer: "0.0.1",//客户端版本号
			RequestParams: "",//请求数据的参数JSON字符串
		},//请求报文
	},
	mutations: {

	},
	actions: {

	},
	modules: {
		websocket,
		loginStore
	},
})