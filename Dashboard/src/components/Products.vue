<template>
  <v-container fluid>
    <v-data-table
          v-bind:headers="headers"
          :items="items"
          hide-actions
          class="elevation-1"
      >
      <template slot="items" scope="props">
        <td>{{ props.item.CodArtigo }}</td>
        <td class="text-xs-right">{{ props.item.DescArtigo }}</td>
        <td class="text-xs-right">{{ props.item.STKAtual }}</td>
      </template>
    </v-data-table>
  </v-container>
</template>

<script>
import Products from '@/services/Products'

export default {
  data () {
    return {
      pagination: {
        sortBy: 'Codigo'
      },
      selected: [],
      headers: [
        {
          text: 'Codigo',
          align: 'left',
          value: 'CodArtigo'
        },
        { text: 'Descrição', value: 'DescArtigo' },
        { text: 'STKAtual', value: 'STKAtual' }
      ],
      items: []
    }
  },
  methods: {
    toggleAll () {
      if (this.selected.length) this.selected = []
      else this.selected = this.items.slice()
    },
    changeSort (column) {
      if (this.pagination.sortBy === column) {
        this.pagination.descending = !this.pagination.descending
      } else {
        this.pagination.sortBy = column
        this.pagination.descending = false
      }
    }
  },
  mounted: function () {
    this.$nextTick(async () => {
      const res = await Products.all()
      this.items = res.data
    })
  }
}
</script>

<style scoped>

</style>

