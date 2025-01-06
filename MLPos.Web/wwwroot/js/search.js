class Search {
    constructor({ searchInput, listElement, cardClassName, hideClassName, lookupClasses }) {
        this.listElement = listElement;
        this.lookupClasses = lookupClasses;
        this.hideClassName = hideClassName;
        this.extractCards(listElement, cardClassName);

        searchInput.addEventListener('input', (e) => {
            e.preventDefault();
            this.filterCards(e.target.value);
        });
    }

    extractCards(listElement, cardClass) {
        this.cards = Array.from(listElement.querySelectorAll(cardClass));
    }

    filterCards(input) {
        this.cards.forEach(card => {
            card.classList.remove(this.hideClassName);
            let lookupValue = '';
            this.lookupClasses.forEach(className => {
                const textElement = card.querySelector(className);
                if(textElement)
                    lookupValue += textElement.innerHTML;
            });

            if (!lookupValue.toLowerCase().match(input.toLowerCase())) {
                card.classList.add(this.hideClassName);
            }
        });
    }
}