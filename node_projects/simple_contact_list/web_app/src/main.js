import Vue from 'vue';
import Vuetify from 'vuetify';
import App from './App.vue';
import VueSession from 'vue-session';
import router from './router';
import vuetify from './plugins/vuetify';

Vue.config.productionTip = false;
Vue.use(Vuetify);
Vue.use(VueSession);

new Vue({
  router,
  vuetify,
  render: h => h(App)
}).$mount('#app')
