import React from "react";
import { Link } from "react-router-dom";

export default () => {
    return (
        <div>
           <h1>Időpont foglaló webes alkalmazás Home</h1>
           <p>Lorem ipsum dolor sit amet consectetur adipisicing elit. Perferendis nobis enim quasi fugit ratione sed? Debitis, eius harum iste excepturi voluptatem asperiores ad nemo fuga numquam sed inventore distinctio amet?</p> 
            <Link to="/contractor/contractor1">Contractor1</Link><br/>
            <Link to="/contractor/contractor2">Contractor2</Link><br/>
        </div>
    );
}