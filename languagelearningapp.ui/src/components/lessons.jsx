import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { languageAppService } from '../services/languageAppService';
import '../css/lessons.css';

export default function Lessons() {
    const [userInfo, setUserInfo] = useState(null);
    const _languageAppService = new languageAppService("https://localhost:7134/api");
    const navigate = useNavigate();

    useEffect(() => {
        const fetchUserInfo = async () => {
            const fetchedUserInfo = await _languageAppService.getUserInfo();
            if (fetchedUserInfo && !fetchedUserInfo.learningLanguage) {
                navigate('/changelanguage');
            } else {
                setUserInfo(fetchedUserInfo);
            }
        };

        fetchUserInfo();
    }, []);

    const stages = [
        { id: 0, title: "Greetings" },
        { id: 1, title: "Food" },
        { id: 2, title: "Numbers" },
    ];

    if (!userInfo) {
        return <div>Loading...</div>;
    }

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