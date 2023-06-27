(function () {
  var numberOfStats = 4;
  var displayedStats = new Array(numberOfStats).fill(null);
  var display = {};

  window.addEventListener("load", function () {
    var wrapper = document.querySelector(".stats");
    // statSetup is generated in DOM (global scope) by the backend
    if (wrapper && statSetup) {
      var setup = [];
      var disposalStatSetup = [];

      for (var i = 0; i < numberOfStats; i++) {
        var element = {
          stat: document.createElement("div"),
          countWrap: document.createElement("div"),
          count: document.createElement("div"),
          suffix: document.createElement("div"),
          name: document.createElement("div"),
        };
        element.stat.classList.add("stat");
        element.stat.appendChild(element.countWrap);
        element.countWrap.classList.add("count-wrap");
        element.countWrap.appendChild(element.count);
        element.count.classList.add("count");
        element.countWrap.appendChild(element.suffix);
        element.suffix.classList.add("suffix");
        element.stat.appendChild(element.name);
        element.name.classList.add("name");
        element.name.innerHTML = "_";
        element.name.style.opacity = 0;
        wrapper.appendChild(element.stat);
        setup.push(element);
        display[i.toString()] = { count: 0 };
        setTimeout(assignNewStat, 160 * i, i, true);
      }

      function onAnimate(count) {
        setup[count].count.innerHTML = Math.floor(display[count.toString()].count);
      }

      function revealStat(count) {
        const tweenTime = Math.random() * 0.4 + 0.8;
        gsap.to(display[count.toString()], {
          duration: tweenTime,
          count: displayedStats[count].count,
          ease: Sine.easeInOut,
          onUpdate: onAnimate,
          onUpdateParams: [count],
        });

        gsap.to(setup[count].suffix, {
          duration: tweenTime / 2,
          opacity: 0,
          ease: Sine.easeIn,
        });

        gsap.to(setup[count].name, {
          duration: tweenTime / 2,
          opacity: 0,
          ease: Sine.easeIn,
          onComplete: changeText,
          onCompleteParams: [count],
        });
      }

      function changeText(count) {
        setup[count].name.innerHTML = displayedStats[count].name;
        setup[count].suffix.innerHTML = displayedStats[count].suffix ? displayedStats[count].suffix : "";
        var tween = {
          duration: 0.4,
          opacity: 1,
          ease: Sine.easeOut,
        };
        gsap.to(setup[count].name, tween);
        gsap.to(setup[count].suffix, tween);
      }

      function assignNewStat(count, fade) {
        if (!disposalStatSetup.length) {
          disposalStatSetup = statSetup.slice();
          displayedStats.forEach(function (item) {
            if (item) {
              disposalStatSetup = disposalStatSetup.filter((setupItem) => setupItem.id != item.id);
            }
          });
        }

        var item = disposalStatSetup.splice(Math.floor(Math.random() * disposalStatSetup.length), 1);
        displayedStats[count] = item[0];
        revealStat(count);
        gsap.to(display[count.toString()], {
          duration: Math.random() * 5 + 3,
          onComplete: assignNewStat,
          onCompleteParams: [count],
        });
        if (fade) {
          gsap.from(setup[count].countWrap, {
            duration: 0.4,
            opacity: 0,
            ease: Sine.easeOut,
          });
        }
      }
    }
  });
})();
