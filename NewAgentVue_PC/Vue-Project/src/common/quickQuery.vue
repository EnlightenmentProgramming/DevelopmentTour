<!--
    作者：offline
    时间：2018-01-15
    描述：快速查询组件
-->
<template>
	<div id="quickQuery">
		快速检索：
		<button @click="search(-1)" :class ="['lastDay-btn',{'cuBtn': currentBtn === -1}]" class="bLightBlue">昨日</button>
		<button @click="search(1)" :class ="['cuDay-btn',{'cuBtn': currentBtn === 1}]" class="bLightBlue">今日</button>
		<button @click="search(7)" :class ="['cuWeek-btn',{'cuBtn': currentBtn === 7}]" class="bLightBlue">本周</button>
		<button @click="search(-7)" :class ="['lastWeek-btn',{'cuBtn': currentBtn === -7}]" class="bLightBlue">上周</button>
		<button @click="search(30)" :class ="['cuMonth-btn',{'cuBtn': currentBtn === 30}]" class="bLightBlue">本月</button>
		<button @click="search(-30)" :class ="['lastWeek-btn',{'cuBtn': currentBtn === -30}]" class="bLightBlue">上月</button>
	</div>
</template>

<script>
import common from '../../extendJS/common.js'
	export default {
		name: 'quickQuery',
		props: {

		},//props The End
		methods: {
			/**
			 * [search 快速搜索]
			 * @return {[type]} [description]
			 */
			search(sign) {
				switch(sign) {
					case -1:
						var lastDay = common.getDateBetween(2);
						this.startDate = lastDay[0];
						this.endDate =  lastDay[1];
						break;
					case 1:
						this.startDate = this.endDate = common.getSystemTime();
						break;
					case -7:
						var lastWeek = common.getWeekStartAndEnd(-1);
						this.startDate = lastWeek[0];
						this.endDate = lastWeek[1];
						break;
					case 7:
						var cuWeek = common.getWeekStartAndEnd(0);
						this.startDate = cuWeek[0];
						this.endDate = cuWeek[1];
						break;
					case -30:
						var lastMonth = common.getDateBetween(4);
						this.startDate = lastMonth[0];
						this.endDate = lastMonth[1];
						break;
					case 30:
						var cuMonth = common.getDateBetween(3);
						this.startDate = cuMonth[0];
						this.endDate = cuMonth[1];
						break;
					default:
						break;
				}//switch The End
				this.currentBtn = sign;
				//父组件通过goSearch方法进行快速搜索
	            this.$emit('goSearch', this.startDate,this.endDate);
	            this.$store.commit("upQueryState",sign);
			},
			/**
			 * [clearSelected 清除选中按钮]
			 * @return {[type]} [description]
			 */
			clearSelected() {
				this.currentBtn = 0;
			},
			selectedBtn(sign){
				this.currentBtn = sign;
			},
		},//methods The End
		computed: {

		},//computed The End
		data() {
			return {
				startDate: common.getSystemTime(),
				endDate: common.getSystemTime(),
				currentBtn: 1,//默认当前选中按钮为当日
			}
		},//data The End
		watch: {

		},//watch The End
	}
</script>

<style>
#quickQuery{
	font-size: 14px;width: 400px;position: absolute;left: 320px;top:4px;
}	
#quickQuery > button {
	width: 50px;height:25px;margin-left: 1px;margin-right: 0;border-radius: 3px;color: #fff;font-size: 12px;line-height: 25px;
}
#quickQuery .cuBtn{background: #da4f49;border: 1px solid #da4f49;}
</style>
