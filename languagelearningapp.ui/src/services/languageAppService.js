import Cookies from 'js-cookie';

export class languageAppService {
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
    
            // Ensure response is not empty before attempting to parse as JSON
            if (res.status === 204 || res.headers.get('Content-Length') === '0') {
                return null;
            }
    
            const contentType = res.headers.get('Content-Type');
            if (contentType && contentType.indexOf('application/json') !== -1) {
                let data = await res.json();
                return data;
            } else if (contentType && contentType.indexOf('text/plain') !== -1) {
                let data = await res.text();
                return data;
            } else {
                return null;
            }
        } catch (err) {
            console.error(`Failed to receive data from server:`, err);
            throw err;
        }
    }

    async register(username, email, password) {
        const url = `${this.url}/User/register`;
        const body = {
            username: username,
            email: email,
            password: password
        };
        
        return await this.#_myFetch(url, 'POST', body);
    }

    async login(username, password) {
        const url = `${this.url}/User/login`;
        const body = {
            username: username,
            password: password
        };
        
        return await this.#_myFetch(url, 'POST', body);
    }

    userIsLoggedIn(){
        return Cookies.get('accessToken') != null;
    }

    logout() {
        Cookies.remove('accessToken');
    }

    async getUserInfo() {
        if (!this.userIsLoggedIn()) {
            return null;
        }

        const url = `${this.url}/User/userInfo`;
        const userInfo = await this.#_myFetch(url, 'GET');
        return userInfo;
    }

    async getLessonInfo(id) {
        const url = `${this.url}/Lessons/lesson?id=${id}`;
        return await this.#_myFetch(url, 'GET');
    }

    async evaluateResponse(lessonId, stageId, promptId, userResponse) {
        const url = `${this.url}/Lessons/evaluate`;
        const body = {
            lessonId: lessonId,
            stageId: stageId,
            promptId: promptId,
            userResponse: userResponse
        };
        return await this.#_myFetch(url, 'POST', body);
    }

    async updateLearningLanguage(language) {
        const url = `${this.url}/User/updateLearningLanguage`;
        const body = { learningLanguage: language };
        const updatedUserInfo = await this.#_myFetch(url, 'POST', body);
        return updatedUserInfo;
    }
}