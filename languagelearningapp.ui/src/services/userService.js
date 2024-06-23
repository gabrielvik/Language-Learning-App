import React, { useEffect } from 'react';
import Cookies from 'js-cookie';

export class userService {
    constructor(baseUrl) {
        this.url = baseUrl;
    }

    #getAccessTokenFromCookies() {
        return Cookies.get('accessToken');
    }

    async #_myFetch(url, method = null, body = null) {
        try {
            method ??= 'GET';
            const accessToken = this.#getAccessTokenFromCookies();
            const headers = {
                'Content-Type': 'application/json',
            };
            if (accessToken) {
                headers['Authorization'] = `Bearer ${accessToken}`;
            }
            let res = await fetch(url, {
                credentials: 'include',
                method: method,
                headers: headers,
                body: body ? JSON.stringify(body) : null
            });

            if (!res.ok) {
                let errorData;
                try {
                    errorData = await res.json();
                } catch (err) {
                    errorData = { message: 'Unknown error' };
                }
                throw { response: { status: res.status, data: errorData } };
            }

            console.log(`\n${method} Request successful @ ${url}`);

            // Ensure response is not empty before attempting to parse as JSON
            if (res.status === 204 || res.headers.get('Content-Length') === '0') {
                return null;
            }

            const contentType = res.headers.get('Content-Type');
            if (contentType && contentType.indexOf('application/json') !== -1) {
                let data = await res.json();
                return data;
            } else {
                return null;
            }
        } catch (err) {
            console.error(`Failed to receive data from server:`, err);
            // throw err;
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

    userIsLoggedIn(){
        if(Cookies.get('accessToken')) {
            return true;
        }
        return false;
    }

    async getUserInfo() {
        const url = `${this.url}/email`;
        
        const userInfo = await this.#_myFetch(url, 'GET');

        return userInfo
    }
}