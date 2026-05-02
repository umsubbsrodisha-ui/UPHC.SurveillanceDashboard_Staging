
//This code(//1.) uses Uses Web Audio API which generates sound mathematically instead of  playing a mp3 file


//1.


//Concept:

//Sound = vibration = wave

//So browser creates wave like:

//sin wave → speaker → sound

let beepInterval = null;
let audioCtx = null;

window.startBeepLoop = function () {
    if (beepInterval) return;

    // Create a sound engine
    //AudioContext = modern browser API for sound
    //webkitAudioContext = fallback for older browsers

    audioCtx = new (window.AudioContext || window.webkitAudioContext)();

    //Run beep repeatedly every X time

    beepInterval = setInterval(() => {
        const oscillator = audioCtx.createOscillator();  //Create sound wave generator . Like a digital “speaker tone generator”
        const gainNode = audioCtx.createGain();  //Controls volume


        //Connect pipeline:

        //Oscillator → Volume → Speaker


        oscillator.connect(gainNode);
        gainNode.connect(audioCtx.destination);



        //Sound pitch(Hz)
        //400 → low tone
        //800 → medium alert
        //1200 → sharp alert 🚨

        oscillator.frequency.value = 1200;  //Create 1200 vibrations per second

      //Shape of sound wave

      //  "sine" → smooth beep
      //  "square" → harsh alarm
      //  "triangle" → softer
        oscillator.type = "sine";

        //Volume(0.0 to 1.0)

        //0.1 = low
        //0.5 = loud

        gainNode.gain.setValueAtTime(0.5, audioCtx.currentTime);

        oscillator.start();  //Start sound

        setTimeout(() => {
            oscillator.stop();
        }, 200);
    }, 1000);   //Stop after 200ms...This creates “beep” instead of continuous sound
};


     //Stop repeating beep
window.stopBeepLoop = function () {
    if (beepInterval) {
        clearInterval(beepInterval);
        beepInterval = null;
    }
    //Turn off sound engine completely
    if (audioCtx) {
        audioCtx.close();
        audioCtx = null;
    }
};


//2.

//For confirmed communicable disease cases this toater appears as a second notification after the first sent suspected notification

window.showToast = (message) => {

    // Create toast container if not exists
    let container = document.getElementById("toast-container");

    if (!container) {
        container = document.createElement("div");
        container.id = "toast-container";
        container.style.position = "fixed";
        container.style.top = "20px";
        container.style.right = "20px";
        container.style.zIndex = "9999";
        document.body.appendChild(container);
    }

    // Create toast
    const toast = document.createElement("div");
    toast.innerText = message;

    toast.style.background = "#dc3545"; // red for confirmed
    toast.style.color = "white";
    toast.style.padding = "10px 15px";
    toast.style.marginTop = "10px";
    toast.style.borderRadius = "5px";
    toast.style.boxShadow = "0 2px 6px rgba(0,0,0,0.2)";
    toast.style.fontSize = "14px";
    toast.style.opacity = "0";
    toast.style.transition = "opacity 0.5s ease";

    container.appendChild(toast);

    // Fade in
    setTimeout(() => {
        toast.style.opacity = "1";
    }, 100);

    // Auto remove after 3 seconds
    setTimeout(() => {
        toast.style.opacity = "0";
        setTimeout(() => {
            toast.remove();
        }, 500);
    }, 3000);
};


//3.    code for trend chart

//keeping chart instance globally
let trendChartInstance = null;

window.renderTrendChart = (labels, confirmed, suspected) => {

    const canvas = document.getElementById('trendChart');
    if (!canvas) return;

    const ctx = canvas.getContext('2d');

    //  DESTROY OLD CHART to avoid one chart created on top of another chart as dashboard
    //is updated using signalr automatically whenever there comes an update

        if (trendChartInstance) {
        trendChartInstance.destroy();
    }

    //  Gradient for Confirmed
    const gradientRed = ctx.createLinearGradient(0, 0, 0, 400);
    gradientRed.addColorStop(0, "rgba(239,68,68,0.6)");
    gradientRed.addColorStop(1, "rgba(239,68,68,0)");

    //  Gradient for Suspected
    const gradientOrange = ctx.createLinearGradient(0, 0, 0, 400);
    gradientOrange.addColorStop(0, "rgba(245,158,11,0.6)");
    gradientOrange.addColorStop(1, "rgba(245,158,11,0)");

trendChartInstance = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [
                {
                    label: 'Confirmed',
                    data: confirmed,
                    borderColor: '#ef4444',
                    backgroundColor: gradientRed,
                    borderWidth: 3,
                    pointRadius: 4,
                    pointHoverRadius: 6,
                    tension: 0.4,
                    fill: true
                },
                {
                    label: 'Suspected',
                    data: suspected,
                    borderColor: '#f59e0b',
                    backgroundColor: gradientOrange,
                    borderWidth: 3,
                    pointRadius: 4,
                    pointHoverRadius: 6,
                    tension: 0.4,
                    fill: true
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,

            animation: {
                duration: 1200,
                easing: 'easeInOutQuart'
            },

            interaction: {
                mode: 'index',
                intersect: false
            },

            plugins: {
                legend: {
                    labels: {
                        font: {
                            size: 14   //  readable on TV
                        }
                    }
                },
                tooltip: {
                    backgroundColor: "#111827",
                    titleColor: "#fff",
                    bodyColor: "#fff",
                    padding: 10
                }
            },

            scales: {
                x: {
                    ticks: {
                        font: {
                            size: 12
                        }
                    },
                    grid: {
                        display: false
                    }
                },
                y: {
                    beginAtZero: true,
                    ticks: {
                        font: {
                            size: 12
                        }
                    },
                    grid: {
                        color: "rgba(0,0,0,0.05)"
                    }
                }
            }
        }
    });
};



//4.  code for disease distribution pie chart

let diseasePieChartInstance = null;
window.renderDiseasePieChart = (labels, data) => {

    const canvas = document.getElementById('diseasePieChart');
    if (!canvas) return;

    const ctx = canvas.getContext('2d');

    //Destroying old chart to avoid memory leak problem
    if (diseasePieChartInstance) {
        diseasePieChartInstance.destroy();
    }

    const colors = [
        "#6366f1",
        "#ef4444",
        "#f59e0b",
        "#10b981",
        "#3b82f6",
        "#8b5cf6"
    ];

  diseasePieChartInstance =  new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: colors,
                borderWidth: 2,
                borderColor: "#fff",
                hoverOffset: 10
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,

            plugins: {
                legend: {
                    position: 'right',
                    labels: {
                        font: {
                            size: 13   //  readable
                        }
                    }
                },
                tooltip: {
                    backgroundColor: "#111827",
                    titleColor: "#fff",
                    bodyColor: "#fff",
                    padding: 10,
                    callbacks: {
                        label: function (context) {
                            let total = context.dataset.data.reduce((a, b) => a + b, 0);
                            let value = context.raw;
                            let percent = ((value / total) * 100).toFixed(1);
                            return `${context.label}: ${value} (${percent}%)`;
                        }
                    }
                }
            }
        }
    });
};


//5.
let uphcBarChartInstance = null;
window.renderUPHCBarChart = (labels, data) => {

    const canvas = document.getElementById('facilityBarChart');
    if (!canvas) return;

    const ctx = canvas.getContext('2d');

    //destroying old instance of chart to avoid memory leak issue
    if (uphcBarChartInstance) {
        uphcBarChartInstance.destroy();
    }


    //  Gradient
    const gradient = ctx.createLinearGradient(0, 0, 400, 0);
    gradient.addColorStop(0, "#3b82f6");
    gradient.addColorStop(1, "#06b6d4");

 uphcBarChartInstance=   new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Cases',
                data: data,
                backgroundColor: gradient,
                borderRadius: 8,
                barThickness: 20
            }]
        },
        options: {
            indexAxis: 'y', //  horizontal

            responsive: true,
            maintainAspectRatio: false,

            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    backgroundColor: "#111827",
                    titleColor: "#fff",
                    bodyColor: "#fff",
                    padding: 10
                }
            },

            scales: {
                x: {
                    beginAtZero: true,
                    grid: {
                        color: "rgba(0,0,0,0.05)"
                    },
                    ticks: {
                        font: {
                            size: 12
                        }
                    }
                },
                y: {
                    grid: {
                        display: false
                    },
                    ticks: {
                        font: {
                            size: 13 //  readable
                        }
                    }
                }
            }
        }
    });
};