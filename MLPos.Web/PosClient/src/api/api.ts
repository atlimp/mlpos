class Api {
    posClientId;

    constructor({ posClientId }: ApiParams) {
        this.posClientId = posClientId;
    }

    async fetchJson(endpoint: string, method: string = 'GET', body: object = {}) {
        const baseUrl = import.meta.env.VITE_API_URL;

        const options: RequestInit = {
            method,
        };

        if (method !== 'GET') {
            options.body = JSON.stringify(body);
            options.headers = [['Content-Type', 'application/json']]
        }


        const response = await fetch(`${baseUrl}${endpoint}`, options);

        if (response.status >= 200 && response.status < 300) {
            return await response.json();
        }

        return null;
    }

    async fetch(endpoint: string, method: string = 'GET') {
        const baseUrl = import.meta.env.VITE_API_URL;

        const response = await fetch(`${baseUrl}${endpoint}`, { method });

        if (response.status >= 200 && response.status < 300) {
            return true;
        }

        return false;
    }

    async getAllProducts(): Promise<Product[]> {
        const response = await this.fetchJson('/api/Product/all');

        if (response) {
            return response as Product[];
        }

        return [];
    }

    async getAllCustomers(): Promise<Customer[]> {
        const response = await this.fetchJson('/api/Customer/all');

        if (response) {
            return response as Customer[];
        }

        return [];
    }

    async getAllPaymentMethods(): Promise<PaymentMethod[]> {
        const response = await this.fetchJson('/api/PaymentMethod/all');

        if (response) {
            return response as PaymentMethod[];
        }

        return [];
    }

    async getActiveTransactions(): Promise<Transaction[]> {
        const response = await this.fetchJson(`/api/Transaction/${this.posClientId}/active`);

        if (response) {
            return response as Transaction[];
        }

        return [];
    }

    async getTransaction(transactionId: number): Promise<Transaction | null> {
        const response = await this.fetchJson(`/api/Transaction/${this.posClientId}/${transactionId}`);

        if (response) {
            return response as Transaction;
        }

        return null;
    }

    async postTransaction(transactionId: number, paymentMethodId: number): Promise<PostedTransaction | null> {
        const response = await this.fetchJson(`/api/Transaction/${this.posClientId}/${transactionId}/post`, 'POST', { paymentMethodId });

        if (response) {
            return response as Transaction;
        }

        return null;
    }

    async deleteTransactionLine(transactionId: number, lineId: number): Promise<Transaction | null> {
        const response = await this.fetchJson(`/api/Transaction/${this.posClientId}/${transactionId}/lines/${lineId}`, 'DELETE');

        if (response) {
            return response as Transaction;
        }

        return null;
    }

    async createTransaction(customerId: number): Promise<Transaction | null> {
        const response = await this.fetchJson(`/api/Transaction/${this.posClientId}`,
            'POST',
            {
                customerId
            }
        );

        if (response) {
            return response as Transaction;
        }

        return null;
    }

    async addTransactionLine(transactionId: number, productId: number): Promise<Transaction | null> {
        const response = await this.fetchJson(
            `/api/Transaction/${this.posClientId}/${transactionId}/Lines`
            , 'POST'
            ,{
                ProductId: productId,
                Quantity: 1
            });

        if (response) {
            return response as Transaction;
        }

        return null;
    }

    async deleteTransaction(transactionId: number): Promise<boolean> {
        return await this.fetch(`/api/Transaction/${this.posClientId}/${transactionId}`, 'DELETE');
    }

    async getActiveTransactionSummaries(): Promise<TransactionSummary[]> {
        const response = await this.fetchJson(`/api/Transaction/${this.posClientId}/active/summary`);

        if (response) {
            return response as TransactionSummary[];
        }

        return [];
    }
}

export default Api;