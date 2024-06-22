import React, { useState } from 'react';
import { userService } from '../services/userService';

export default function Login() {
    const _userService = new userService("https://localhost:7134/api/User");

    const [form, setForm] = useState({
        username: '',
        userEmail: '',
        userPassword: '',
        confirmPassword: ''
    });
    const [errors, setErrors] = useState({
        username: false,
        userEmail: false,
        userPassword: false,
        confirmPassword: false
    });
    const [serverErrors, setServerErrors] = useState({});
    const [registrationSuccess, setRegistrationSuccess] = useState(false);

    const handleChange = (e) => {
        const { id, value } = e.target;
        setForm({ ...form, [id]: value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        const newErrors = {
            username: form.username === '',
            userEmail: form.userEmail === '',
            userPassword: form.userPassword === '',
            confirmPassword: form.confirmPassword !== form.userPassword
        };

        setErrors(newErrors);
        setServerErrors({});

        const noErrors = !Object.values(newErrors).some(error => error);

        if (noErrors) {
            try {
                await _userService.register(form.username, form.userEmail, form.userPassword);
                setRegistrationSuccess(true);
            } catch (error) {
                if (error.response && error.response.status === 400) {
                    setServerErrors(error.response.data);
                } else {
                    alert('Registration failed.');
                }
            }
        }
    };

    return (
        <div className="container px-4 py-4" id="register">
            {registrationSuccess && (
                <div className="alert alert-success" role="alert">
                    Registration successful!
                </div>
            )}
            <form className="needs-validation" noValidate onSubmit={handleSubmit}>
                <h2 className="pb-2 border-bottom">Register yourself</h2>
                <p>If you register yourself you can edit the data in the database.</p>
                <div className="row mb-3">
                    <div className="col">
                        <label htmlFor="username" className="form-label">Username</label>
                        <input type="text" className={`form-control ${errors.username ? 'is-invalid' : ''}`} id="username" value={form.username} onChange={handleChange} required />
                        <div className="invalid-feedback">
                            Username is required
                        </div>
                        {serverErrors.Username && <div className="invalid-feedback d-block">{serverErrors.Username[0]}</div>}
                    </div>
                </div>
                <div className="row mb-3">
                    <div className="col">
                        <label htmlFor="userEmail" className="form-label">Email</label>
                        <input type="email" className={`form-control ${errors.userEmail ? 'is-invalid' : ''}`} id="userEmail" value={form.userEmail} onChange={handleChange} required />
                        <div className="invalid-feedback">
                            Email is required
                        </div>
                        {serverErrors.Email && <div className="invalid-feedback d-block">{serverErrors.Email[0]}</div>}
                    </div>
                </div>
                <div className="row mb-3">
                    <div className="col-md-6">
                        <label htmlFor="userPassword" className="form-label">Password</label>
                        <input type="password" className={`form-control ${errors.userPassword ? 'is-invalid' : ''}`} id="userPassword" value={form.userPassword} onChange={handleChange} required />
                        <div className="invalid-feedback">
                            Password is required
                        </div>
                        {serverErrors.Password && <div className="invalid-feedback d-block">{serverErrors.Password[0]}</div>}
                    </div>
                    <div className="col-md-6">
                        <label htmlFor="confirmPassword" className="form-label">Confirm Password</label>
                        <input type="password" className={`form-control ${errors.confirmPassword ? 'is-invalid' : ''}`} id="confirmPassword" value={form.confirmPassword} onChange={handleChange} required />
                        <div className="invalid-feedback">
                            Passwords do not match
                        </div>
                    </div>
                </div>
                <button className="w-100 btn btn-primary btn-lg my-4" type="submit">Register</button>
            </form>
        </div>
    )
}
