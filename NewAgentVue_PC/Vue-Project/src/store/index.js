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

Vue.use(Vuex)

export default new Vuex.Store({
	state: {
		
	},
	mutations: {

	},
	actions: {

	},
	modules: {
		websocket,
	},
})