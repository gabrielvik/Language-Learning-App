import React, { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { languageAppService } from '../services/languageAppService';
import '../css/lessons.css';

export default function Lesson() {
    const { lessonId } = useParams();
    const _languageAppService = new languageAppService("https://localhost:7134/api");
    const [lessonInfo, setLessonInfo] = useState(null);

    useEffect(() => {
        const fetchLessonInfo = async () => {
            const fetchedLessonInfo = await _languageAppService.getLessonInfo(lessonId);
            setLessonInfo(fetchedLessonInfo);
        };
    
        fetchLessonInfo();
    }, []);
    
    return (
        <></>
    );
}
