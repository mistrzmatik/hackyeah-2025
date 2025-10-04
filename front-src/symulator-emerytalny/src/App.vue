<script setup>
import { ref } from 'vue'
import { VaButtonToggle, VaButton } from 'vuestic-ui'

const expectedRetirementValue = ref(4000)
const age = ref(25)
const gender = ref(0)
const grossSalary = ref(7500)
const workFromAge = ref(20)
const retirementAge = ref(65)
const savedInZUS = ref(0)

const includeSickLeave = ref(false)


</script>

<template>

  <VaInput
      v-model="expectedRetirementValue"
      placeholder="0.00"
      type="number"
      label="Wysokość emerytury w przyszłości"
    >
      <template #appendInner>
        <span>PLN</span>
      </template>
    </VaInput>

   <VaInput
      v-model="age"
      label="Wiek"
      type="number"
      required-mark
    >
      <template #appendInner>
        <span>lat</span>
      </template>
    </VaInput>

  <VaButtonToggle
    v-model="gender"
    :options="[
      { label: 'Kobieta', value: 0 },
      { label: 'Mężczyzna', value: 1 },
    ]"
  />

    <VaInput
      v-model="grossSalary"
      label="Wysokość wynagrodzenia brutto"
      placeholder="0.00"
      type="number"
      required-mark
    >
      <template #appendInner>
        <span>PLN</span>
      </template>
    </VaInput>

    <VaInput
      v-model="workFromAge"
      label="Rok rozpoczęcia pracy"
      type="number"
      required-mark
    >
      <template #appendInner>
        <span>lat</span>
      </template>
    </VaInput>

    <VaInput
      v-model="retirementAge"
      label="Planowany rok zakończenia aktywności zawodowej"
      type="number"
      required-mark
    >
      <template #appendInner>
        <span>lat</span>
      </template>
    </VaInput>

    <VaInput
      v-model="savedInZUS"
      label="Wysokość zgromadzonych środków na koncie i na subkoncie w ZUS"
      placeholder="0.00"
      type="number"
    >
      <template #appendInner>
        <span>PLN</span>
      </template>
    </VaInput>

    <VaCheckbox
      v-model="includeSickLeave"
      class="mb-6"
      label="Uwzględniaj możliwość zwolnień lekarskich"
    />
    <VaPopover message="Półroczne zwolnienie lekarskie to około –1,3% składki emerytalnej.">
      <VaIcon
        name="info"
      />
    </VaPopover>

    <!-- -->

  <VaButton color="primary">Symuluj</VaButton>
</template>
<style>
</style>