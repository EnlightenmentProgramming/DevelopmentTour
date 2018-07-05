<template>
	<div class="show hide" id="animate">
		<div class="loading an_Box">
			<div class="load-state">
				<div class="ball"></div>
				<div class="ball"></div>
				<div class="ball"></div>
				<!-- <div class="ball"></div>
				<div class="ball"></div>
				<div class="ball"></div> -->
			</div>
		</div>
	</div>
</template>
<script>

export default{
	name: "loading",
	data() {
		return {

		}
	},
	computed: {
		laodSwitch:{
			get(){
				return this.$store.state.laodSwitch;
			}
		}
	},
	methods: {
		init(){
			$(".an_Box").css({"left":($(window).width() - 400)/2 + "px","top":($(window).height() - 300)/2 + "px"});
		},
		running(){
			$("#animate").removeClass("hide");
		},
		stopping(){
			$("#animate").addClass('hide');
		},
	},
	mounted() {
		this.init();
		//使用方法：开：this.$store.commit("laodSwitchInfo",true);关：this.$store.commit("laodSwitchInfo",false);
		$(window).resize(()=>{
			this.init();
		})
	},
	watch:{
		laodSwitch:{
			handler(val,old){
				if(val){
					this.running();
				}else{
					this.stopping();
				}
			}
		}
	}
}
</script>
<style scoped>
	.hide{display: none;}
	.show{position: absolute;top: 0;left: 0;width: 100%;height: 100%;z-index: 100;background-color: #000;opacity: 0.6;}
	.an_Box{width: 400px;text-align: center;height: 300px;position: absolute;top: 145px;line-height: 300px;left: 40%;}
	.load-state .ball {height: 10px;width: 10px;margin-right: 10px;background: #FFF;display: inline-block;-moz-border-radius: 1000px;-webkit-border-radius: 1000px;border-radius: 1000px;-webkit-animation: ballBounce 0.3s alternate infinite;-moz-animation: ballBounce 0.3s alternate infinite;animation: ballBounce 0.3s alternate infinite;-webkit-animation-timing-function: cubic-bezier(0.2, 0.4, 0.3, 1);-moz-animation-timing-function: cubic-bezier(0.2, 0.4, 0.3, 1);animation-timing-function: cubic-bezier(0.2, 0.4, 0.3, 1);-moz-transition-duration: 0.3s;-o-transition-duration: 0.3s;-webkit-transition-duration: 0.3s;transition-duration: 0.3s;
	}
	.load-state .ball:nth-child(2n) {-webkit-animation-delay: 0.05s;-moz-animation-delay: 0.05s;animation-delay: 0.05s;}
	.load-state .ball:nth-child(3n) {-webkit-animation-delay: 0.1s;-moz-animation-delay: 0.1s;animation-delay: 0.1s;}
	.load-state .ball:last-child {margin-right: 0;}
	.button.load-btn.done .ball {opacity: 0;}
	@-webkit-keyframes ballBounce {
	  0% {
	    -moz-transform: translateY(25%);
	    -ms-transform: translateY(25%);
	    -webkit-transform: translateY(25%);
	    transform: translateY(25%);
	  }

	  100% {
	    -moz-transform: translateY(-75%);
	    -ms-transform: translateY(-75%);
	    -webkit-transform: translateY(-75%);
	    transform: translateY(-75%);
	  }
	}
	@-moz-keyframes ballBounce {
	  0% {
	    -moz-transform: translateY(25%);
	    -ms-transform: translateY(25%);
	    -webkit-transform: translateY(25%);
	    transform: translateY(25%);
	  }

	  100% {
	    -moz-transform: translateY(-75%);
	    -ms-transform: translateY(-75%);
	    -webkit-transform: translateY(-75%);
	    transform: translateY(-75%);
	  }
	}
	@keyframes ballBounce {
	  0% {
	    -moz-transform: translateY(25%);
	    -ms-transform: translateY(25%);
	    -webkit-transform: translateY(25%);
	    transform: translateY(25%);
	  }

	  100% {
	    -moz-transform: translateY(-75%);
	    -ms-transform: translateY(-75%);
	    -webkit-transform: translateY(-75%);
	    transform: translateY(-75%);
	  }
	}
</style>