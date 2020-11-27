<template>
  <v-app>
    <nav>
      <v-navigation-drawer app :key="drawer" dark v-model="drawer" left>
        <v-toolbar dense>
          <v-list name="title">
            <v-list-tile name="titleText">
              <v-list-tile-title name="titleTitleText" class="title">SmartContacts</v-list-tile-title>
            </v-list-tile>
          </v-list>
        </v-toolbar>
        <v-divider></v-divider>
        <!-- Menu Links -->
        <v-list name="nav">
          <v-list-item name="contax" to="/" exact>
            <v-list-tile-action>
              <v-icon class="pa-2">contacts</v-icon>
            </v-list-tile-action>
            <v-list-tile-content name="tileContent">Contacts</v-list-tile-content>
          </v-list-item>
          <v-list-item to="/addContact">
            <v-list-tile-action>
              <v-icon class="pa-2">add</v-icon>
            </v-list-tile-action>
            <v-list-tile-content>Add Contact</v-list-tile-content>
          </v-list-item>
        </v-list>

        <template v-slot:append>
          <div class="pa-2">
            <v-btn @click="logout" depressed width="100%" color="error">
              <v-icon class="pa-2">exit_to_app</v-icon>Logout
            </v-btn>
          </div>
        </template>
      </v-navigation-drawer>
    </nav>

    <v-content>
      <!-- Display view pages here based on route -->
      <router-view></router-view>
    </v-content>
  </v-app>
</template>

<script>
  import router from './router/index'
  // import { EventBus } from './router/index'

  export default {
    name: "App",
    data() {
      return {
        drawer: false
      };
    },
      
    mounted() {
      // EventBus.$on('loggedIn', (data) => {
      //   this.drawer = data;
      // })
      if (!router.app.$session.exists() && !router.app.$session.has('jwt')) {
        this.drawer = false;
      } else {
        this.drawer = true;
      }
    },

    methods: {
      async logout() {
        router.app.$session.clear();
        router.app.$session.destroy();
        this.$router.push('/login');
        this.drawer = false;
      }
    }
  };
</script>