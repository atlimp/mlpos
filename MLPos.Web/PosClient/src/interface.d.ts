interface Entity {
    id: number;
    dateInserted: Date;
    dateUpdated: Date;
}

enum ProductType {
    Item = 0,
    Service = 1
}

interface Product extends Entity {
    name: string;
    description: string;
    type: ProductType;
    image: string;
    price: number;
}

interface Customer extends Entity {
    name: string;
    email: string;
    image: string;
}