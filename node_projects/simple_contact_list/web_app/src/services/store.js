import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex);

export const store = new Vuex.Store({
    state: {
        logged_in: null
    },

    mutations: {
        change(state, logged_in) {
            state.logged_in = logged_in;
        }
    },

    getters: {
        logged_in: state => state.logged_in
    }
});