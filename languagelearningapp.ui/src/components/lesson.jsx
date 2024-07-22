import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { languageAppService } from '../services/languageAppService';
import '../css/lesson.css';

export default function Lesson() {
    const { lessonId } = useParams();
    const _languageAppService = new languageAppService("https://localhost:7134/api");
    const [lessonInfo, setLessonInfo] = useState(null);
    const [selectedLessonType, setSelectedLessonType] = useState(null);
    const [currentPromptIndex, setCurrentPromptIndex] = useState(0);
    const [userResponse, setUserResponse] = useState("");
    const [feedback, setFeedback] = useState(null);

    useEffect(() => {
        const fetchLessonInfo = async () => {
            const fetchedLessonInfo = await _languageAppService.getLessonInfo(lessonId);
            setLessonInfo(fetchedLessonInfo);
            
        };

        fetchLessonInfo();
    }, [lessonId]);

    const handleLessonTypeChange = (event) => {
        setSelectedLessonType(event.target.value);
        setCurrentPromptIndex(0);
        setUserResponse("");
        setFeedback(null);
    };

    const handleResponseChange = (event) => {
        setUserResponse(event.target.value);
    };

    const handleSubmit = async (event) => {
        event.preventDefault();

        if (!lessonInfo || !selectedLessonType) return;

        const selectedLesson = lessonInfo.find(lesson => lesson.name === selectedLessonType);
        const stageId = lessonInfo.indexOf(selectedLesson);
        const promptId = currentPromptIndex;

        const response = await _languageAppService.evaluateResponse(lessonId, stageId, promptId, userResponse);
        setFeedback(response);
    };

    const handleNextPrompt = () => {
        setCurrentPromptIndex(currentPromptIndex + 1);
        setUserResponse("");
        setFeedback(null);
    };

    return (
        <div className="lesson-container">
            {lessonInfo && (
                <div>
                    <h1>{lessonInfo.name}</h1>
                    <label>Select Lesson Type:</label>
                    <select onChange={handleLessonTypeChange} value={selectedLessonType}>
                        <option value="">Select...</option>
                        {lessonInfo.map((lesson, index) => (
                            <option key={index} value={lesson.name}>
                                {lesson.name}
                            </option>
                        ))}
                    </select>
                </div>
            )}

            {selectedLessonType && lessonInfo && (
                <div className="prompt-container">
                    <h2>{lessonInfo.find(lesson => lesson.name === selectedLessonType).name}</h2>
                    
                    <label>{lessonInfo.find(lesson => lesson.name === selectedLessonType).prompt[currentPromptIndex]}</label>
                    
                    {feedback === null ? (
                        <form onSubmit={handleSubmit}>
                            <input
                                type="text"
                                value={userResponse}
                                onChange={handleResponseChange}
                            />
                            <button type="submit">Submit</button>
                        </form>
                    ) : (
                        <div>
                            <p>Your response: {userResponse}</p>
                            <p>Feedback: {feedback}</p>
                            {currentPromptIndex < lessonInfo.find(lesson => lesson.name === selectedLessonType).prompt.length - 1 ? (
                                <button onClick={handleNextPrompt}>Next</button>
                            ) : (
                                <p>All prompts completed!</p>
                            )}
                        </div>
                    )}
                </div>
            )}
        </div>
    );
}
