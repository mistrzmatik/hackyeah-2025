<script setup>
import { ref, watch } from 'vue'
import { VaButtonToggle, VaButton } from 'vuestic-ui'
import VChart from 'vue-echarts';
import { postPrognozujEmeryture } from './services/api'

const WOMEN = 0;
const MAN = 1;

const AGE_FOR_WOMAN = 60;
const AGE_FOR_MAN = 65;

const step = ref(0)

const steps = [
  { label: 'Oczekiwania' },
  { label: 'Symulacja' },
  { label: 'Podsumowanie' }
]

const expectedRetirement = ref(8769.08)
const yearOfBirth = ref(1990)
const gender = ref(0)
const grossSalary = ref(7500)
const workingAges = ref([25, 60])
const savedInZUS = ref(0)
const includeSickLeave = ref(false)

const isLoading = ref(false)
const predictedRetirement = ref(0)

watch(gender, async (after) => {
  if (after == MAN && workingAges.value[1] == AGE_FOR_WOMAN) {
    workingAges.value[1] = AGE_FOR_MAN;
  }else if(after == WOMEN && workingAges.value[1] == AGE_FOR_MAN){
    workingAges.value[1] = AGE_FOR_WOMAN;
  }
});

watch(expectedRetirement, async (after) => {
  firstReportOptions.value.series[0].data[0] = after;
});

const firstReportOptions = ref({
  xAxis: {
    type: 'value'
  },
  yAxis: {
    type: 'category',
    data: [''],
    inverse: true,
  },
  series: [
    {
      realtimeSort: true,
      name: 'Oczekiwana wysokość emerytury',
      type: 'bar',
      data: [expectedRetirement.value],
      label: {
        show: true,
        valueAnimation: true,
        formatter: (params) => `${params.value.toLocaleString('pl-PL', {
          minimumFractionDigits: 2,
          maximumFractionDigits: 2
        })} zł`
      },
      
    },
    {
      realtimeSort: true,
      name: 'Średnia emerytura brutto',
      type: 'bar',
      data: [4045.20],
      label: {
        show: true,
        valueAnimation: true,
        formatter: (params) => `${params.value.toLocaleString('pl-PL', {
          minimumFractionDigits: 2,
          maximumFractionDigits: 2
        })} zł`
      },
    },
    {
      realtimeSort: true,
      name: 'Minimalna emerytura',
      type: 'bar',
      data: [1878.91],
      label: {
        show: true,
        valueAnimation: true,
        formatter: (params) => `${params.value.toLocaleString('pl-PL', {
          minimumFractionDigits: 2,
          maximumFractionDigits: 2
        })} zł`
      },
    }
  ],
  color: [
    "#00993f",
    "#00416e",
    "#bec3ce"
  ],
  legend: {
    show: true
  },
  animationDuration: 0,
  animationDurationUpdate: 200,
  animationEasing: 'linear',
  animationEasingUpdate: 'linear'
});

watch([gender, yearOfBirth, workingAges, grossSalary, includeSickLeave], async ([newGender, newYearOfBirth, newWorkingAges, newGrossSalary, newIncludeSickLeave]) => {
  await debouncedFetchReport();
})

watch(step, async (after) => {
  await debouncedFetchReport();
});

let debounceTimer = null
const debouncedFetchReport = () => {
  isLoading.value = true;
  if (debounceTimer) clearTimeout(debounceTimer)
  debounceTimer = setTimeout(async () => {
    await fetchReport()
  }, 500) // 500 ms opóźnienia
}

const fetchReport = async () => {
  isLoading.value = true;

  try {
    var response = await postPrognozujEmeryture({
      czyMezczyzna: gender.value == MAN,
      oczekiwanaEmerytura: expectedRetirement.value,
      wiek: new Date().getFullYear() - yearOfBirth.value,
      wiekPrzejsciaNaEmeryture: workingAges.value[1],
      wynagrodzeniaBrutto: {2025: grossSalary.value},
      sredniaIlosciDniL4WRoku: includeSickLeave.value ? 12 : 0
    });

    secondReportOptions.value.xAxis[0].data = Object.keys(response.przewidywanaEmerytura)
    secondReportOptions.value.series[0].data = Object.entries(response.przewidywanaEmerytura).map(([_, value]) => value.wysokoscRzeczywista)
    secondReportOptions.value.series[1].data = Object.entries(response.przewidywanaEmerytura).map(([_, value]) => value.wysokoscUrealniona)

    predictedRetirement.value = response.przewidywanaEmerytura[yearOfBirth.value + workingAges.value[1]];
  } finally {
    isLoading.value = false;
  }
}

const toPolishZlString = (num) => {
  return num.toLocaleString('pl-PL', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  });
}

const secondReportOptions = ref({
  tooltip: {
    trigger: 'axis',
    axisPointer: {
      type: 'cross',
      label: {
        backgroundColor: '#6a7985'
      }
    },
  },
  legend: {
    data: ['Wysokość rzeczywista', 'Wysokość urealniona']
  },
  color: [
    "#00993f",
    "#00416e",
    "#bec3ce"
  ],
  toolbox: {
    feature: {
      saveAsImage: {}
    }
  },
  xAxis: [
    {
      type: 'category',
      boundaryGap: false,
      data: []
    }
  ],
  yAxis: [
    {
      type: 'value'
    }
  ],
  series: [
    {
      name: 'Wysokość rzeczywista',
      type: 'line',
      areaStyle: {},
      emphasis: {
        focus: 'series'
      },
      data: []
    },
    {
      name: 'Wysokość urealniona',
      type: 'line',
      areaStyle: {},
      emphasis: {
        focus: 'series'
      },
      data: []
    }
  ]
});

</script>

<template>

  <h2 class="va-h2" style="margin-bottom: 20px; margin-top: 20px;">
    Symulator emerytalny
  </h2>

   <h6 class="va-h6" style="margin-bottom: 20px; margin-top: 20px;">
     Zaplanuj swoją przyszłość już dziś!
Skorzystaj z Symulatora emerytalnego, aby sprawdzić, jak Twoje decyzje zawodowe i finansowe wpływają na wysokość przyszłej emerytury. Wprowadź podstawowe informacje, a narzędzie pokaże Ci realne scenariusze Twojej sytuacji po zakończeniu pracy. Dzięki temu łatwiej zaplanujesz bezpieczną i spokojną przyszłość.
  </h6>

<VaStepper
    v-model="step"
    :steps="steps"
    linear
  >
    <template #step-content-0>
      <div>
    <h6 class="va-h6">Jakiej wysokości emeryturę chciałbyś otrzymywać w przyszłości?</h6>
    <div style="display: flex; margin-bottom: 20px;">
      <VaInput
        v-model="expectedRetirement"
        placeholder="0.00"
        type="number"
        input-class="text-right"
        style="width: 150px; margin-right: 10px;"
      >
        <template #appendInner>
          <span>zł</span>
        </template>
      </VaInput>
      <div style="display: flex; align-items: center; margin-right: 5px;">brutto</div>
      <VaDivider vertical />
      <div style="display: flex; flex-direction: column; flex: 1;">
        <VaSlider v-model="expectedRetirement" min="0" max="20000" style="flex: 1;"/>
        <div>
          <span style="font-size: 12px;">0 zł</span>
          <span style="font-size: 12px; float: right;">20 000 zł</span>
        </div>
      </div>
    </div>
    <VChart style="width: 100%; height: 300px;" :option="firstReportOptions" autoresize />
     <VaAlert
      color="secondary"
      outline
      class="mb-6"
    >
      <template #icon>
        <VaIcon
          name="lightbulb"
          color="secondary"
        />
      </template>
      Czy wiesz, że najwyższą emeryturę w Polsce otrzymuje mieszkaniec <b>województwa śląskiego</b>, wysokość jego emerytury to <b>51 350,57 zł</b>, pracował przez <b>62 lata</b>, nie był nigdy na zwolnieniu lekarskim.
    </VaAlert>
    </div>
    </template>
    <template #step-content-1>

      <div style="display: flex; margin-bottom: 20px;">
        
        <div style="flex: 1;">
          <h6 class="va-h6">Płeć</h6>
          <VaButtonToggle
            v-model="gender"
            preset="secondary"
            border-color="secondary"
            :options="[
              { label: 'Kobieta',  icon: 'woman', value: 0 },
              { label: 'Mężczyzna',  icon: 'man', value: 1 },
            ]"
          />
        </div>
        <div style="flex: 1;">
          <h6 class="va-h6">Rok urodzenia</h6>
          <VaInput
              v-model="yearOfBirth"
              type="number"
              required-mark
            />
        </div>
        <div style="flex: 1;">
          <h6 class="va-h6">Wynagrodzenie brutto</h6>
          <VaInput
            v-model="grossSalary"
            placeholder="0.00"
            type="number"
            required-mark
          >
            <template #appendInner>
              <span>zł</span>
            </template>
          </VaInput>
        </div>
      </div>
      <div style="margin-bottom: 20px;">

       <h6 class="va-h6">Okres pracy</h6>
       <VaSlider
          v-model="workingAges"
          range
          track-label-visible
          style="margin-top: 30px;"
          min="15"
          max="100"
        >
          <template #trackLabel="{ value, order }">
            <VaChip
              size="small"
              outline
            >
              {{ order === 0 ? 'od ' + value : 'do ' + value }} lat
            </VaChip>
          </template>
        </VaSlider>

      </div>
    <div>
      
      <VaCheckbox
        v-model="includeSickLeave"
        class="mb-6"
        label="Uwzględniaj średnią liczbę dni zwolnienia lekarskiego w roku"
      />
      <VaPopover message="W Polsce średnia liczba dni zwolnienia lekarskiego na pracownika wynosi około 11-12 dni rocznie (zgodnie z danymi z 2023 i 2024 roku)." >
        <VaIcon name="info" style="padding-left: 10px; line-height: 8px;" />
      </VaPopover>

     <VaDivider/>

    <div style="margin-bottom: 20px; display: flex;">
      <h6 class="va-h6" style="flex: 3; margin-top: 15px;">Przewidywana wysokość ostatniej wypłaty w {{ yearOfBirth + workingAges[1] }} roku wynosi</h6>
      <VaInnerLoading :loading="isLoading" style="flex: 1;">
        <h3 class="va-h3" style="text-align: right;">{{ toPolishZlString(predictedRetirement?.przewidywanaWyplata ?? 0) }} zł</h3>
      </VaInnerLoading>
    </div>

    <div style="margin-bottom: 20px; display: flex;">
      <h6 class="va-h6" style="flex: 3; margin-top: 15px;">Przewidywana wysokość emerytury w {{ yearOfBirth + workingAges[1] }} roku wynosi</h6>
      <VaInnerLoading :loading="isLoading" style="flex: 1;">
        <h3 class="va-h3" style="text-align: right;">{{ toPolishZlString(predictedRetirement?.wysokoscRzeczywista ?? 0) }} zł</h3>
      </VaInnerLoading>
    </div>

    <div style="margin-bottom: 20px; display: flex;">
      <h6 class="va-h6" style="flex: 3; margin-top: 15px;">Stopa zastąpienia</h6>
      <VaInnerLoading :loading="isLoading" style="flex: 1;">
        <h3 class="va-h3" style="text-align: right;">{{ toPolishZlString(predictedRetirement?.rr ?? 0) }} %</h3>
      </VaInnerLoading>
    </div>

      <!-- <VaInput
          v-model="savedInZUS"
          label="Wysokość zgromadzonych środków na koncie i na subkoncie w ZUS"
          placeholder="0.00"
          type="number"
        >
          <template #appendInner>
            <span>zł</span>
          </template>
        </VaInput> -->

    </div>
  
    </template>
    <template #step-content-2>
      <div>
        <VChart style="width: 100%; height: 300px;" :option="secondReportOptions" autoresize />
      </div>
    </template>
  </VaStepper>
</template>
<style>
.text-right {
  text-align: right;
}
</style>