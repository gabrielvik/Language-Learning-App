import React from 'react';
import { userService } from '../services/userService';

export default function Home(){
    const _userService = new userService("https://localhost:7134/api/User");
    console.log(_userService.userIsLoggedIn())
    if(_userService.userIsLoggedIn()){
      _userService.getUserInfo();
    }

    return (
        <div class="container px-4 py-4" id="home">
        <div class="bg-body-tertiary p-5 rounded">
          <div class="col-sm-8 py-5 mx-auto">
            <h1 class="display-5 fw-normal">Language Learning App</h1>
              <p class="fs-5">On this page you will be able to learn languages with help of openAI</p>
            
              <p>To be written more on. Enjoy!</p>
          </div>
        </div>
      </div>
    )
}