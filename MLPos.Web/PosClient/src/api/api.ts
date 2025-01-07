class Api {
    async fetchJson(endpoint: string) {
        const baseUrl = import.meta.env.VITE_API_URL;

        const response = await fetch(`${baseUrl}${endpoint}`);

        if (response.status >= 200 && response.status < 300) {
            return await response.json();
        }

        return null;
    }

    async getAllProducts(): Promise<[Product?]> {
        const response = await this.fetchJson('/api/Product/all');

        if (response) {
            return response as [Product];
        }

        return [];
    }

    async getAllCustomers(): Promise<[Customer?]> {
        const response = await this.fetchJson('/api/Customer/all');

        if (response) {
            return response as [Customer];
        }

        return [];
    }
}

export default Api;