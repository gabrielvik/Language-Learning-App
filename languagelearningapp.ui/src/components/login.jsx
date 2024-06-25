import React, { useState } from 'react';
import { languageAppService } from '../services/languageAppService';

export default function Login() {
    const _languageAppService = new languageAppService("https://localhost:7134/api");

    const [form, setForm] = useState({
        username: '',
        userPassword: ''
    });
    const [errors, setErrors] = useState({
        username: false,
        userPassword: false
    });
    const [serverErrors, setServerErrors] = useState({});
    const [loginSuccess, setLoginSuccess] = useState(false);

    const handleChange = (e) => {
        const { id, value } = e.target;
        setForm({ ...form, [id]: value });
        //setLoginSuccess(false); // Hide success message when form is edited
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        const newErrors = {
            username: form.username === '',
            userPassword: form.userPassword === ''
        };

        setErrors(newErrors);
        setServerErrors({});

        const noErrors = !Object.values(newErrors).some(error => error);

        if (noErrors) {
            try {
                await _languageAppService.login(form.username, form.userPassword);
                setLoginSuccess(true);
            } catch (error) {
                if (error.response && error.response.status === 400) {
                    setServerErrors(error.response.data);
                } else {
                    // Handle other server errors if necessary
                }
            }
        }
    };

    return (
        <div className="container px-4 py-4" id="sign-in">
            <div className="form-signin w-100 m-auto">
                {loginSuccess && (
                    <div className="alert alert-success" role="alert">
                        Login successful!
                    </div>
                )}
                <form className="needs-validation" noValidate onSubmit={handleSubmit}>
                    <h1 className="h3 mb-3 fw-normal">Please sign in</h1>

                    <div className="form-floating">
                        <input type="text" className={`form-control ${errors.username ? 'is-invalid' : ''}`} id="username" placeholder="Username" value={form.username} onChange={handleChange} required />
                        <label htmlFor="username">Username</label>
                        <div className="invalid-feedback">
                            Username is required
                        </div>
                        {serverErrors.Username && <div className="invalid-feedback d-block">{serverErrors.Username[0]}</div>}
                    </div>

                    <div className="form-floating">
                        <input type="password" className={`form-control ${errors.userPassword ? 'is-invalid' : ''}`} id="userPassword" placeholder="Password" value={form.userPassword} onChange={handleChange} required />
                        <label htmlFor="userPassword">Password</label>
                        <div className="invalid-feedback">
                            Password is required
                        </div>
                        {serverErrors.Password && <div className="invalid-feedback d-block">{serverErrors.Password[0]}</div>}
                    </div>

                    <div className="form-check text-start my-3">
                        <input className="form-check-input" type="checkbox" value="remember-me" id="flexCheckDefault" />
                        <label className="form-check-label" htmlFor="flexCheckDefault">
                            Remember me
                        </label>
                    </div>
                    <button className="btn btn-primary w-100 py-2" type="submit">Sign in</button>
                </form>
            </div>
        </div>
    );
}
