﻿import React from "react";

export const PlusIcon = ({ className }: { className?: string }) =>
    <svg className={className} height="448pt" viewBox="0 0 448 448" width="448pt">
        <path d="m408 184h-136c-4.417969 0-8-3.582031-8-8v-136c0-22.089844-17.910156-40-40-40s-40 17.910156-40 40v136c0 4.417969-3.582031 8-8 8h-136c-22.089844 0-40 17.910156-40 40s17.910156 40 40 40h136c4.417969 0 8 3.582031 8 8v136c0 22.089844 17.910156 40 40 40s40-17.910156 40-40v-136c0-4.417969 3.582031-8 8-8h136c22.089844 0 40-17.910156 40-40s-17.910156-40-40-40zm0 0"/>
    </svg>;


export const EditIcon = ({ className }: { className?: string }) =>
    <svg className={className} version="1.1" xmlns="http://www.w3.org/2000/svg" x="0px" y="0px" viewBox="0 0 383.947 383.947">
    <g>
        <polygon points="0,303.947 0,383.947 80,383.947 316.053,147.893 236.053,67.893"/>
        <path d="M377.707,56.053L327.893,6.24c-8.32-8.32-21.867-8.32-30.187,0l-39.04,39.04l80,80l39.04-39.04
            C386.027,77.92,386.027,64.373,377.707,56.053z"/>
    </g>
    </svg>;

export const DeleteIcon = ({ className }: { className?: string }) =>
    <svg className={className} version="1.1" x="0px" y="0px" width="510px" height="510px" viewBox="0 0 510 510">
        <g id="cancel">
            <path d="M255,0C114.75,0,0,114.75,0,255s114.75,255,255,255s255-114.75,255-255S395.25,0,255,0z M382.5,346.8l-35.7,35.7
                L255,290.7l-91.8,91.8l-35.7-35.7l91.8-91.8l-91.8-91.8l35.7-35.7l91.8,91.8l91.8-91.8l35.7,35.7L290.7,255L382.5,346.8z"/>
        </g>
    </svg>;
    
export const InfoIcon = ({ className }: { className?: string }) =>
    <svg className={className} version="1.1" x="0px" y="0px" viewBox="0 0 426.667 426.667">
    <g>
        <path d="M213.333,0C95.467,0,0,95.467,0,213.333s95.467,213.333,213.333,213.333S426.667,331.2,426.667,213.333S331.2,0,213.333,0
            z M234.667,320H192V192h42.667V320z M234.667,149.333H192v-42.667h42.667V149.333z"/>
    </g>
    </svg>;
    
export const RightArrow = ({ className }: { className?: string }) =>
    <svg className={className} version="1.1" x="0px" y="0px" viewBox="0 0 512 512">
        <g>
            <path d="M506.134,241.843c-0.006-0.006-0.011-0.013-0.018-0.019l-104.504-104c-7.829-7.791-20.492-7.762-28.285,0.068
                c-7.792,7.829-7.762,20.492,0.067,28.284L443.558,236H20c-11.046,0-20,8.954-20,20c0,11.046,8.954,20,20,20h423.557
                l-70.162,69.824c-7.829,7.792-7.859,20.455-0.067,28.284c7.793,7.831,20.457,7.858,28.285,0.068l104.504-104
                c0.006-0.006,0.011-0.013,0.018-0.019C513.968,262.339,513.943,249.635,506.134,241.843z"/>
        </g>
    </svg>; 
    
export const LogoutIcon = ({ className }: { className?: string }) =>
    <svg className={className} version="1.1" x="0px" y="0px"
         width="122.775px" height="122.776px" viewBox="0 0 122.775 122.776">
    <g>
        <path d="M86,28.074v-20.7c0-3.3-2.699-6-6-6H6c-3.3,0-6,2.7-6,6v3.9v78.2v2.701c0,2.199,1.3,4.299,3.2,5.299l45.6,23.601
            c2,1,4.4-0.399,4.4-2.7v-23H80c3.301,0,6-2.699,6-6v-32.8H74v23.8c0,1.7-1.3,3-3,3H53.3v-30.8v-19.5v-0.6c0-2.2-1.3-4.3-3.2-5.3
            l-26.9-13.8H71c1.7,0,3,1.3,3,3v11.8h12V28.074z"/>
        <path d="M101.4,18.273l19.5,19.5c2.5,2.5,2.5,6.2,0,8.7l-19.5,19.5c-2.5,2.5-6.301,2.601-8.801,0.101
            c-2.399-2.399-2.1-6.4,0.201-8.8l8.799-8.7H67.5c-1.699,0-3.4-0.7-4.5-2c-2.8-3-2.1-8.3,1.5-10.3c0.9-0.5,2-0.8,3-0.8h34.1
            c0,0-8.699-8.7-8.799-8.7c-2.301-2.3-2.601-6.4-0.201-8.7C95,15.674,98.9,15.773,101.4,18.273z"/>
    </g>
    </svg>;  
    
export const ProfileIcon = ({ className }: { className?: string }) =>
    <svg className={className} version="1.1" x="0px" y="0px"
         width="45.532px" height="45.532px" viewBox="0 0 45.532 45.532">
    <g>
        <path d="M22.766,0.001C10.194,0.001,0,10.193,0,22.766s10.193,22.765,22.766,22.765c12.574,0,22.766-10.192,22.766-22.765
            S35.34,0.001,22.766,0.001z M22.766,6.808c4.16,0,7.531,3.372,7.531,7.53c0,4.159-3.371,7.53-7.531,7.53
            c-4.158,0-7.529-3.371-7.529-7.53C15.237,10.18,18.608,6.808,22.766,6.808z M22.761,39.579c-4.149,0-7.949-1.511-10.88-4.012
            c-0.714-0.609-1.126-1.502-1.126-2.439c0-4.217,3.413-7.592,7.631-7.592h8.762c4.219,0,7.619,3.375,7.619,7.592
            c0,0.938-0.41,1.829-1.125,2.438C30.712,38.068,26.911,39.579,22.761,39.579z"/>
    </g>
    </svg>;
