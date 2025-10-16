function registerTransitionEnd(elementSelector, dotNetObject) {
    const element = document.querySelector(elementSelector);
    if (element) {
        element.addEventListener('transitionend', function() {
            dotNetObject.invokeMethodAsync('OnTransitionEnd');
        });
    }
}
