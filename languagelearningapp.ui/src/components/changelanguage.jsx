import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { languageAppService } from '../services/languageAppService';
//import '../css/changeLanguage.css';

export default function ChangeLanguage() {
    const [selectedLanguage, setSelectedLanguage] = useState('');
    const _languageAppService = new languageAppService("https://localhost:7134/api");
    const navigate = useNavigate();

    const handleLanguageChange = (event) => {
        setSelectedLanguage(event.target.value);
    };

    const handleLanguageSubmit = async () => {
        if (selectedLanguage) {
            await _languageAppService.updateLearningLanguage(selectedLanguage);
            navigate('/lessons');
        }
    };

    return (
        <div className="container mt-4">
            <h2 className="mb-4 text-center">Select Your Learning Language</h2>
            <div className="form-group">
                <label htmlFor="languageSelect">Choose a language:</label>
                <select id="languageSelect" className="form-control" value={selectedLanguage} onChange={handleLanguageChange}>
                    <option value="" disabled>Select a language</option>
                    <option value="French">French</option>
                    <option value="German">German</option>
                    <option value="Spanish">Spanish</option>
                </select>
            </div>
            <button className="btn btn-primary mt-3" onClick={handleLanguageSubmit} disabled={!selectedLanguage}>
                Save Language
            </button>
        </div>
    );
}
