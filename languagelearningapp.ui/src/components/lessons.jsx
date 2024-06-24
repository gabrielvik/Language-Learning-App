import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { userService } from '../services/userService';
import '../css/lessons.css';

export default function Lessons() {
    const _userService = new userService("https://localhost:7134/api/User");
    const [userInfo, setUserInfo] = useState(null);

    useEffect(() => {
        const fetchUserInfo = async () => {
            const fetchedUserInfo = await _userService.getUserInfo();
            setUserInfo(fetchedUserInfo);
        };

        fetchUserInfo();
    }, []);

    const stages = [
        { id: 1, title: "Greetings" },
        { id: 2, title: "Food" },
        { id: 3, title: "Numbers" },
    ];
            
    return (
        <div className="container mt-4">
            <h2 className="mb-4 text-center">Progression Ladder</h2>
            <div className="row">
                {stages.map(stage => (
                    <div key={stage.id} className="col-md-4 mb-3">
                        <Link to={`/lesson/${stage.id}`} className="card lesson-card text-center p-4">
                            <div className="card-body">
                                <h4 className="card-title">{stage.title}</h4>
                                <p className="card-text text-muted">Click to start this lesson</p>
                            </div>
                        </Link>
                    </div>
                ))}
            </div>
        </div>
    );
}