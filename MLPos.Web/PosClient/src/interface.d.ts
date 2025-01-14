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
interface PaymentMethod extends Entity {
    name: string;
    description: string;
    image: string;
}

interface TransactionLine {
    id: number;
    amount: number;
    quantity: number;
    product: Product;
}

interface Transaction {
    id: number;
    customer: Customer;
    lines: TransactionLine[];
}
interface PostedTransaction {
    id: number;
    customer: Customer;
}

interface TransactionSummary {
    id: number;
    posClientId: number;
    customerName: string;
    customerImage: string;
    totalAmount: number;
}

interface ApiParams {
    posClientId: number;
}

interface PosProps {
    refreshTransactions: () => void;
}

interface TransactionListProps {
    transactions: TransactionSummary[];
    refreshTransactions: () => void;
}

interface LineDisplayProps {
    lines: TransactionLine[];
    onDeleteLine: (id: number) => void;
}

interface ControlPanelProps {
    transaction: Transaction;
    onDeleteTransaction: () => void;
    onAddProduct: () => void;
    onFinishTransaction: () => void;
}
interface ProductSelectProps {
    onSelectProduct: (id: number) => void;
}
interface CustomerSelectProps {
    onSelectCustomer: (id: number) => void;
}

interface PaymentMethodSelectProps {
    onSelectPaymentMethod: (id: number) => void;
    replacers: { [key: string]: string; };
}