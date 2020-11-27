<template>
  <v-container class="ma-2 text-center">
    <v-row class="fill-height" align="start" justify="start">
      <!-- POP UP FORM STARTS HERE -->
      <v-container>
        <v-fade-transition>
        <v-card v-show="dialog" dark>
          <v-card-title>
            <span class="headline">Contact</span>
          </v-card-title>
          <v-card-text>
            <v-container>
              <v-row>
                <v-col cols="12" sm="6" md="4">
                  <v-text-field label="Name*" required v-model="dlgName">{{ dlgName }}</v-text-field>
                </v-col>
                <v-col cols="12" sm="6" md="4">
                  <v-text-field label="Last name*" v-model="dlgLastName" required>{{ dlgLastName }}</v-text-field>
                </v-col>
                <v-col cols="12">
                  <v-text-field label="Email" v-model="dlgEmail">{{ dlgEmail }}</v-text-field>
                </v-col>
                <v-col cols="12">
                  <v-text-field label="Phone number" required v-model="dlgPhoneNumber">{{ dlgPhoneNumber }}</v-text-field>
                </v-col>
                <p v-show="error" align="center">{{ errorMsg }}</p>
              </v-row>
            </v-container>
          </v-card-text>
            <v-container fluid>
              <v-row align="center" justify="center">
                <v-btn
                  color="success"
                  class="ma-2"
                  fab
                  @click="updateContact(selectedId)"
                  dark
                >
                  <v-icon>done</v-icon>
                </v-btn>
                <v-btn
                  dark
                  color="red"
                  @click="cancel"
                  fab
                >
                  <v-icon>close</v-icon>
                </v-btn>
                <v-container>
              <v-row align="end" justify="end" class="px-3">
                <v-btn
                  @click="deleteContact(selectedId)"
                  color="error"
                  text
                  fab>
                  <v-icon>delete</v-icon>
                </v-btn>
              </v-row>
            </v-container>
              </v-row>
            </v-container>
            
        </v-card>
        </v-fade-transition>
      </v-container>
      <!-- tiles init here -->
      <template v-for="c in contacts">
        <v-col
          :key="c._id"
          cols="12"
          md="3"
        >
          <v-hover v-slot:default="{ hover }">
            <v-card
              id="tile"
              :elevation="hover ? 12 : 2"
              :class="{ 'on-hover': hover }"
              class="ma-auto"
              dark
            >
                <v-card-title class="title white--text">
                    <template>
                      <v-row
                        class="fill-height flex-column"
                        justify="space-between"
                      >
                      <v-col cols="auto">
                      <div>
                        <p class="ma-auto body-1 text-left">{{ c.lastName }}</p>
                        <p class="ma-0 body-2 font-weight-bold text-left">
                          {{ c.name }}
                        </p>
                        <p class="caption ma-auto font-weight-medium font-italic text-left">
                          {{ c.phoneNumber }}
                        </p>
                        <p class="caption ma-auto font-weight-medium font-italic text-left">
                          {{ c.email }}
                        </p>
                      </div>
                      </v-col>
                      <v-col class="ma-0">
                        <v-btn small fab text dark v-bind="attrs" v-on="on" v-on:click.stop="showDialog(c._id)"><v-icon>create</v-icon></v-btn>
                      </v-col>
                      </v-row>
                    </template>
              </v-card-title>
            </v-card>
          </v-hover>
        </v-col>
      </template>
    </v-row>
  </v-container>
</template>

<style>
#tile {
  transition: opacity .4s ease-in-out;
}

#tile:not(.on-hover) {
  opacity: 0.6;
 }
</style>


<script>
import api from '../services/services'
import router from '../router/index'

export default {
  name: 'home',
  props: [],
  components: {
  },

  data() {
    return {
      contacts: [],
      dialog: false,
      dlgName: '',
      dlgLastName: '',
      dlgPhoneNumber: '',
      dlgEmail: '',
      on: null,
      attrs: null,
      selectedId: null,
      email: null,
      error: '',
      errorMsg: ''
    };
  },

  mounted() {
    this.getAllContacts();
    this.contacts = []
  },

  methods: {
    async getAllContacts() {
      let params = {
        headers: {
          'x-access-token': router.app.$session.get('jwt')
        },
        params: {
          'userId': router.app.$session.get('userId')
        }
      }
      
      const res = await api.getAllContacts(params);
      res.data.forEach(e => {
        this.contacts.push(e);
      });
      this.contacts.sort(function(a, b) {
        var textA = a.lastName.toUpperCase();
        var textB = b.lastName.toUpperCase();
        return (textA < textB) ? -1 : (textA > textB) ? 1 : 0;
      });
    },

    showDialog(key) {
      var dlgContact = this.contacts.find(c => {
        return c._id == key;
      })

      this.dlgName = dlgContact.name;
      this.dlgLastName = dlgContact.lastName;
      this.dlgPhoneNumber = dlgContact.phoneNumber;
      this.dlgEmail = dlgContact.email;
      this.selectedId = dlgContact._id;

      this.dialog = true;
    },

    // UPDATE CONTACT
    updateContact(id) {
      let params = {
        name: this.dlgName,
        lastName: this.dlgLastName,
        email: this.dlgEmail,
        phoneNumber: this.dlgPhoneNumber,
        userId: router.app.$session.get('userId'),
        accessToken: router.app.$session.get('jwt')
      }

      api.putUpdateContact(id, params, (err, res) => {
        if(err) {
          this.error = true;
          this.errorMsg = 'There has been an error!';
        } else {
          console.log(res)
          this.error = true;
          this.errorMsg = 'Contact updated!';
          var index = this.contacts.map(function(item) { return item._id; }).indexOf(id);
          this.contacts[index].name = res.data.contact.name;
          this.contacts[index].lastName = res.data.contact.lastName;
          this.contacts[index].phoneNumber = res.data.contact.phoneNumber;
          this.contacts[index].email = res.data.contact.email;
          // this.contacts = newContacts;
        }

        return res;
      })
    },

    // DELETE CONTACT
    deleteContact(id) {
      let config = {
        headers: {
          'x-access-token': router.app.$session.get('jwt')
        }
      }
        
      let body = {
        id: id
      }

      api.postDeleteContact(body, config, (err, res) => {
        if (err) {
          this.error = true;
          this.errorMsg = 'Could not delete contact.';
        } else {
          this.error = true;
          this.errorMsg = 'Contact deleted, please refresh page.';
          var removeIndex = this.contacts.map(function(item) { return item._id; }).indexOf(id);
          this.contacts.splice(removeIndex, 1);
          this.dialog = false;
        }
        
        return res;
      })
    },

    cancel(){
      this.dialog = false;
      this.$emit('close');
    }
  }
}
</script>
<style lang="scss" scoped>
</style>