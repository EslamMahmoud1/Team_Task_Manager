const endpoints = {
    personalInfo: '/UpdateProfile/SavePersonalInfo',
    education: '/UpdateProfile/SaveEducation',
    skills: '/UpdateProfile/SaveSkills',
    submit: '/UpdateProfile/SubmitProfile',
    photo: '/UpdateProfile/UploadPhoto'
};

function postForm(url, formData) {

    const response = fetch(url,{
            method: 'POST',
            body: formData
    });
     if (!response.ok) 
        throw new Error(`HTTP ${response.status}`);

    return response.json();
}

function getValue(elementId)
{
    return document.getElementById(elementId)?.value || '';
}
function collectPersonalInfo() {
    return({
        firstName : getValue('PersonalInfo_FirstName'),
        lastName : getValue('PersonalInfo_LastName'),
        phone : getValue('PersonalInfo_Phone'),
        location : getValue('PersonalInfo_Location'),
        dateOfBirth : getValue('PersonalInfo_DateOfBirth'),
        headline: getValue('PersonalInfo_Headline'),
        bio: getValue('PersonalInfo_Bio')
    })
};
function savePersonalInfo()
{
        debugger;
    const data = collectPersonalInfo()
    const response = postForm(endpoints.personalInfo, data)
    console.log('Personal info saved:', collectPersonalInfo(response));
    console.log(data)
}

document.getElementById('nextToSection2')?.addEventListener('click', () => savePersonalInfo());
