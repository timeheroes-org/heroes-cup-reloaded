function initCarousel(homePanels, homeIndicators) {
  var hasBeenNavigated = false;
  var timeout = 0;

  if (homePanels && homeIndicators) {
    homeIndicators.forEach(function (indicator, idx) {
      indicator.addEventListener("click", function () {
        hasBeenNavigated = true;
        clearTimeout(timeout);
        setActiveElement(idx);
      });
    });

    function setActiveElement(activeIdx, shortTime) {
      var count = activeIdx >= homeIndicators.length ? 0 : activeIdx;
      homeIndicators.forEach(function (_indicator, idx) {
        var panel = homePanels[idx];
        var indicator = homeIndicators[idx];
        if (panel.classList.contains("leave")) {
          panel.classList.remove("leave");
        }
        if (idx === count) {
          panel.classList.add("active");
          indicator.classList.add("active");
        } else {
          if (panel.classList.contains("active")) {
            panel.classList.add("leave");
            panel.classList.remove("active");
            indicator.classList.remove("active");
          }
        }
      });
      if (!hasBeenNavigated) {
        timeout = setTimeout(setActiveElement, shortTime ? 2500 : 5000, count + 1);
      }
    }
    setActiveElement(0, true);
  }
}
