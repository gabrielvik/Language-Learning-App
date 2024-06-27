import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { languageAppService } from '../services/languageAppService';

export default function Header() {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const _languageAppService = new languageAppService("https://localhost:7134/api");
  const navigate = useNavigate();

  useEffect(() => {
    const checkLoginStatus = async () => {
      const loggedIn = await _languageAppService.userIsLoggedIn();
      setIsLoggedIn(loggedIn);
    };

    checkLoginStatus();
  }, []);

  const handleLogout = () => {
    _languageAppService.logout();
    setIsLoggedIn(false);
    navigate('/login');
  };

  return (
    <div className="container">
      <header className="d-flex flex-wrap justify-content-center py-3 mb-4 border-bottom">
        <Link to="/" className="d-flex align-items-center mb-3 mb-md-0 me-md-auto link-body-emphasis text-decoration-none">
          <span className="fs-4">Language Learning App</span>
        </Link>

        <ul className="nav nav-pills">
          <li className="nav-item">
            <Link to="/" className="nav-link">Home</Link>
          </li>
          {!isLoggedIn && (
            <>
              <li className="nav-item">
                <Link to="/register" className="nav-link">Register</Link>
              </li>
              <li className="nav-item">
                <Link to="/login" className="nav-link">Login</Link>
              </li>
            </>
          )}
          {isLoggedIn && (
            <>
              <li className="nav-item">
                <Link to="/changelanguage" className="nav-link">Change Language</Link>
              </li>
              <li className="nav-item">
                <button onClick={handleLogout} className="nav-link btn btn-link">Logout</button>
              </li>
            </>
          )}
        </ul>
      </header>
    </div>
  );
}