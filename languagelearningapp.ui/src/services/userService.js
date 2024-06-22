export class userService {
    constructor(baseUrl) {
        this.url = baseUrl;
    }

    async #_myFetch(url, method = null, body = null) {
        try {
            method ??= 'GET';
            let res = await fetch(url, {
                credentials: 'include',
                method: method,
                headers: { 'Content-Type': 'application/json' },
                body: body ? JSON.stringify(body) : null
            });

            if (!res.ok) {
                let errorData = await res.json();
                throw { response: { status: res.status, data: errorData } };
            }

            console.log(`\n${method} Request successful @ ${url}`);
            let data = await res.json();
            return data;
        } catch (err) {
            console.log(`Failed to receive data from server: ${err.message}`);
            throw err;
        }
    }

    async register(username, email, password) {
        const url = `${this.url}/register`;
        const body = {
            username: username,
            email: email,
            password: password
        };
        
        return await this.#_myFetch(url, 'POST', body);
    }

    async login(username, password) {
        const url = `${this.url}/login`;
        const body = {
            username: username,
            password: password
        };
        
        return await this.#_myFetch(url, 'POST', body);
    }
}