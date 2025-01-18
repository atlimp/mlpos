class AdminHandlers {
    constructor() {

    }

    initEventHandlers() {
        const navbar = document.querySelector('#navbar');

        const hamburger = document.querySelector('#hamburger')

        if (hamburger) {
            hamburger.addEventListener('click', (e) => {
                this.toggleVisible(navbar);
            });
        }
    }

    toggleVisible(e) {
        e.classList.toggle('hidden');
    }
}

(() => {
    (new AdminHandlers()).initEventHandlers();
})()