import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { languageAppService } from '../services/languageAppService';
import '../css/lesson.css';

export default function Lesson() {
    const { lessonId } = useParams();
    const _languageAppService = new languageAppService("https://localhost:7134/api");
    const [lessonInfo, setLessonInfo] = useState(null);
    const [selectedLessonType, setSelectedLessonType] = useState(null);
    const [selectedLessonTypeId, setSelectedLessonTypeId] = useState(0);
    const [currentPromptIndex, setCurrentPromptIndex] = useState(0);
    const [userResponse, setUserResponse] = useState("");
    const [feedback, setFeedback] = useState(null);
    const [userInfo, setUserInfo] = useState(null);

    useEffect(() => {
        const fetchLessonInfo = async () => {
            const fetchedLessonInfo = await _languageAppService.getLessonInfo(lessonId);
            setLessonInfo(fetchedLessonInfo);
        };

        const fetchUserInfo = async () => {
            const fetchedUserInfo = await _languageAppService.getUserInfo();
            setUserInfo(fetchedUserInfo);
        };

        fetchUserInfo();
        fetchLessonInfo();
    }, [lessonId]);

    const handleLessonTypeChange = (event) => {
        const selectedOption = event.target.options[event.target.selectedIndex];
        const selectedId = Number(selectedOption.getAttribute('data-id'));
        setSelectedLessonTypeId(selectedId);
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

    const hasCompletedPrompt = (lessonId, promptId) => {
        console.log("promptId:", promptId, "Type:", typeof promptId);
        console.log("learnedLessons[lessonId]:", userInfo.learnedLessons[lessonId]);
        console.log("Array item type:", typeof userInfo.learnedLessons[lessonId][0]);
        console.log("Includes check:", userInfo.learnedLessons[lessonId].includes(promptId));
        
        return (
            userInfo &&
            userInfo.learnedLessons &&
            userInfo.learnedLessons[lessonId] &&
            userInfo.learnedLessons[lessonId].includes(promptId)
        );
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
                            <option data-id={index} value={lesson.name}>
                                {lesson.name}
                            </option>
                        ))}
                    </select>
                </div>
            )}

            {selectedLessonType && lessonInfo && (
                <div className="prompt-container">
                    <h2>{selectedLessonType}</h2>
                    <label>{lessonInfo.find(lesson => lesson.name === selectedLessonType).prompt[currentPromptIndex]}</label>

                    {hasCompletedPrompt(lessonId, selectedLessonTypeId) ? (
                        <div>
                            <p>This prompt is already completed. Do you want to do it again?</p>
                            <button onClick={() => setFeedback(null)}>Redo</button>
                        </div>
                    ) : feedback === null ? (
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
