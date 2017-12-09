<template>

  <div class="elevation-1 pa-2">
    <div class="pointer" @click="expand=!expand">
      <v-layout>
        <v-flex>
          <slot name="header"></slot>
        </v-flex>
        <v-spacer></v-spacer>
        <v-flex class="text-xs-right">
          <v-icon class="text-xs-right expand_arrow" :class="[{active:expand}]"> keyboard_arrow_down </v-icon>
        </v-flex>
      </v-layout>
    </div>
    <div class="expand" :class="[{active:expand}]">
      <v-divider> </v-divider>
    <slot name="content"></slot>
    </div>
  </div>

</template>

<script>
export default {
  data () {
    return {
      expand: true
    }
  }
}
</script>

<style scoped>

.expand {
  overflow: hidden;
  height: 0px;
}

.pointer {
  cursor: pointer;
}

.expand_arrow {
  transform: rotate(0deg);
  transition: transform 0.1s;
}

.expand_arrow.active {
  transform: rotate(180deg);
}

.expand.active {
  height: auto;
}

@keyframes anim1 {
  0%{
    transform: rotate(0deg);
  } 
  100%{
    transform: rotate(180deg);
  }
}

</style>
