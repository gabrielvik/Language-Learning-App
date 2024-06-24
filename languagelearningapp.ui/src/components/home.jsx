import React, { useEffect, useState } from 'react';
import { userService } from '../services/userService';
import { Link } from 'react-router-dom';

export default function Home() {
    const [userInfo, setUserInfo] = useState(null);
    const _userService = new userService("https://localhost:7134/api/User");

    useEffect(() => {
        const fetchUserInfo = async () => {
            const fetchedUserInfo = await _userService.getUserInfo();
            setUserInfo(fetchedUserInfo);
        };

        fetchUserInfo();
    }, []);

    return (
        <div className="container-fluid min-vh-100 d-flex align-items-center justify-content-center" style={{ paddingTop: '5rem' }}>
            <div className="card text-center shadow p-5 bg-body rounded" style={{ maxWidth: '40rem' }}>
                <div className="card-body">
                    {userInfo ? (
                        <>
                            <h1 className="card-title display-5 fw-bold">Welcome Back, {userInfo.username}!</h1>
                            <p className="card-text lead">Great to see you again! Let's continue your language learning journey.</p>
                            <Link to="/lessons" className="btn btn-primary btn-lg mt-4">
                                Continue Learning
                            </Link>
                        </>
                    ) : (
                        <>
                            <h1 className="card-title display-5 fw-normal">Welcome to our Language Learning App!</h1>
                            <p className="card-text lead">Join us today and start learning a new language in an interactive and fun way.</p>
                            <div className="d-flex justify-content-center mt-4">
                                <Link to="/register" className="btn btn-success btn-lg me-3">
                                    Register Now
                                </Link>
                                <Link to="/login" className="btn btn-outline-primary btn-lg">
                                    Already Registered? Log In
                                </Link>
                            </div>
                        </>
                    )}
                    <footer className="footer mt-5">
                        <p className="text-muted">Powered by OpenAI, immerse yourself in learning with cutting-edge technology.</p>
                    </footer>
                </div>
            </div>
        </div>
    );
}