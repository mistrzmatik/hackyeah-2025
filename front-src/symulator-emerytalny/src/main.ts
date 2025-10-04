import { createVuestic } from 'vuestic-ui'
import 'vuestic-ui/css'
import './assets/main.css'

import { createApp } from 'vue'
import App from './App.vue'

import "echarts";

const app = createApp(App).use(createVuestic({
      config: {
        colors: {
          variables: {
            // Default colors
            primary: "#00993f",
            secondary: "#bec3ce",
            success: "#00993f",
            info: "#00416e",
            danger: "#f05e5e",
            warning: "#ffc200",
            gray: "#bec3ce",
            dark: "#000000",
          },
        },
        i18n: {
          next: "Następny",
          back: "Wróć",
          finish: "Zakończ"
        },
      },
    }))

app.mount('#app')