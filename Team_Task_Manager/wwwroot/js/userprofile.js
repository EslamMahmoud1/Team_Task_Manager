(function () {
    'use strict';

    const draftKey = 'updateProfileDraft';
    let currentStep = 1;
    let tempProfile = readTempProfile() || emptyProfile();

    const endpoints = {
        personalInfo: '/UpdateProfile/SavePersonalInfo',
        education: '/UpdateProfile/SaveEducation',
        skills: '/UpdateProfile/SaveSkills',
        submit: '/UpdateProfile/SubmitProfile',
        photo: '/UpdateProfile/UploadPhoto'
    };

    const sections = [
        null,
        document.getElementById('section-1'),
        document.getElementById('section-2'),
        document.getElementById('section-3')
    ];

    const dots = [
        null,
        document.getElementById('dot-1'),
        document.getElementById('dot-2'),
        document.getElementById('dot-3')
    ];

    const labels = [
        null,
        document.getElementById('label-1'),
        document.getElementById('label-2'),
        document.getElementById('label-3')
    ];

    const progressFill = document.getElementById('progress-fill');
    const skillEntriesContainer = document.getElementById('skill-entries');
    const skillsJson = document.getElementById('SkillsJson');

    function emptyProfile() {
        return {
            PersonalInfo: {},
            Educations: [],
            Skills: { SkillEntries: [], SkillNames: [] }
        };
    }

    function readTempProfile() {
        try {
            return JSON.parse(sessionStorage.getItem(draftKey) || '');
        } catch {
            return null;
        }
    }

    function persistTempProfile() {
        sessionStorage.setItem(draftKey, JSON.stringify(tempProfile));
    }

    function clearTempProfile() {
        sessionStorage.removeItem(draftKey);
        tempProfile = emptyProfile();
    }

    function token() {
        return document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
    }

    async function postForm(url, formData) {
        if (token()) {
            formData.append('__RequestVerificationToken', token());
        }

        const response = await fetch(url, {
            method: 'POST',
            body: formData,
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        });

        if (!response.ok) {
            throw new Error(`HTTP ${response.status}`);
        }

        return response.json();
    }

    async function postJson(url, data) {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token(),
                'X-Requested-With': 'XMLHttpRequest'
            },
            body: JSON.stringify(data)
        });

        if (!response.ok) {
            throw new Error(`HTTP ${response.status}`);
        }

        return response.json();
    }

    function collectPersonalInfo() {
        return {
            FirstName: value('#PersonalInfo_FirstName'),
            LastName: value('#PersonalInfo_LastName'),
            Phone: value('#PersonalInfo_Phone'),
            Location: value('#PersonalInfo_Location'),
            DateOfBirth: value('#PersonalInfo_DateOfBirth') || null,
            Headline: value('#PersonalInfo_Headline'),
            Bio: value('#PersonalInfo_Bio'),
            ProfilePictureUrl: value('#ProfilePictureUrlValue') || tempProfile.PersonalInfo?.ProfilePictureUrl || ''
        };
    }

    function personalInfoFormData() {
        const personalInfo = collectPersonalInfo();
        const formData = new FormData();

        Object.entries(personalInfo).forEach(([key, val]) => {
            formData.append(key, val ?? '');
        });

        return formData;
    }

    function collectSkills() {
        const entries = collectSkillEntries();
        syncSkillsJson();

        return {
            SkillEntries: entries,
            SkillNames: entries.map(entry => entry.Name).filter(Boolean)
        };
    }

    function skillsFormData() {
        const currentSkills = collectSkills();
        const formData = new FormData();

        formData.append('SkillsJson', JSON.stringify(currentSkills.SkillEntries));

        return formData;
    }

    function collectEducationEntries() {
        return [...document.querySelectorAll('#education-entries .education-entry')].map(entry => {
            const index = entry.dataset.index;
            const field = name =>
                entry.querySelector(`[name="Educations[${index}].${name}"]`)?.value?.trim() || '';

            return {
                Id: parseInt(entry.dataset.dbId || '0', 10) || null,
                Institution: field('Institution'),
                Degree: field('Degree'),
                FieldOfStudy: field('FieldOfStudy'),
                GraduationYear: parseInt(field('GraduationYear'), 10) || 0,
                Description: field('Description')
            };
        });
    }

    function collectSkillEntries() {
        return [...document.querySelectorAll('#skill-entries .skill-entry')].map(entry => {
            const index = entry.dataset.index;
            const field = name =>
                entry.querySelector(`[name="Skills.SkillEntries[${index}].${name}"]`)?.value?.trim() || '';

            return {
                SkillId: parseInt(entry.dataset.skillId || '0', 10) || null,
                Name: field('Name'),
                ProficiencyLevel: field('ProficiencyLevel'),
                YearsOfExperience: parseInt(field('YearsOfExperience'), 10) || 0,
                AdditionalNotes: field('AdditionalNotes')
            };
        });
    }

    function collectFullProfile() {
        return {
            PersonalInfo: collectPersonalInfo(),
            Educations: collectEducationEntries(),
            Skills: collectSkills()
        };
    }

    function storeCurrentSection() {
        if (currentStep === 1) {
            tempProfile.PersonalInfo = collectPersonalInfo();
        } else if (currentStep === 2) {
            tempProfile.Educations = collectEducationEntries();
        } else {
            tempProfile.Skills = collectSkills();
        }

        persistTempProfile();
    }

    function value(selector) {
        return document.querySelector(selector)?.value?.trim() || '';
    }

    function setValue(selector, val) {
        const element = document.querySelector(selector);
        if (element) {
            element.value = val ?? '';
        }
    }

    function normalizeResult(result) {
        return {
            success: result?.success === true || result?.isSuccess === true,
            message: result?.message || result?.value || '',
            errors: result?.errors || []
        };
    }

    function clearErrors(section) {
        section?.querySelectorAll('.field-error').forEach(span => span.textContent = '');
        section?.querySelectorAll('.is-invalid').forEach(input => input.classList.remove('is-invalid'));

        const summary = section?.querySelector('.error-summary');
        if (summary) {
            summary.innerHTML = '';
            summary.style.display = 'none';
        }
    }

    function showErrors(section, errors) {
        const messages = Array.isArray(errors)
            ? errors
            : Object.values(errors || {});

        if (messages.length === 0) {
            messages.push('Unable to validate this section. Please check the form and try again.');
        }

        const summary = section?.querySelector('.error-summary') || createSummary(section);
        if (summary) {
            summary.innerHTML = messages.map(message => `<div>${escapeHtml(String(message))}</div>`).join('');
            summary.style.display = 'block';
            summary.scrollIntoView({ behavior: 'smooth', block: 'center' });
        }
    }

    function createSummary(section) {
        if (!section) {
            return null;
        }

        const summary = document.createElement('div');
        summary.className = 'error-summary';
        section.insertBefore(summary, section.children[1] || null);
        return summary;
    }

    function setLoading(button, isLoading) {
        if (!button) {
            return;
        }

        button.disabled = isLoading;
        button.classList.toggle('btn--loading', isLoading);
    }

    function goToStep(step) {
        if (sections[currentStep]) {
            sections[currentStep].style.display = 'none';
        }

        currentStep = step;

        if (sections[step]) {
            sections[step].style.display = 'block';
            sections[step].scrollIntoView({ behavior: 'smooth', block: 'start' });
        }

        for (let i = 1; i <= 3; i++) {
            if (!dots[i] || !labels[i]) {
                continue;
            }

            dots[i].className = 'progress-dot';
            dots[i].textContent = i;

            if (i < step) {
                dots[i].classList.add('progress-dot--done');
                dots[i].textContent = '✓';
            }

            if (i === step) {
                dots[i].classList.add('progress-dot--active');
            }

            labels[i].classList.toggle('active', i <= step);
        }

        if (progressFill) {
            progressFill.style.width = ['0%', '0%', '50%', '100%'][step];
        }
    }

    async function validatePersonalInfo(advance) {
        const section = sections[1];
        const button = document.getElementById('nextToSection2');

        clearErrors(section);
        setLoading(button, true);

        try {
            const result = normalizeResult(await postForm(endpoints.personalInfo, personalInfoFormData()));
            if (result.success) {
                tempProfile.PersonalInfo = collectPersonalInfo();
                persistTempProfile();

                if (advance) {
                    goToStep(2);
                }

                return true;
            }

            showErrors(section, result.errors);
            return false;
        } catch (error) {
            console.error('[Personal Info] AJAX validation error:', error);
            showErrors(section, ['An error occurred validating personal info.']);
            return false;
        } finally {
            setLoading(button, false);
        }
    }


    async function validateEducation(advance) {
        const section = sections[2];
        const button = document.getElementById('nextToSection3');

        clearErrors(section);
        setLoading(button, true);

        try {
            const entries = collectEducationEntries();
            const result = normalizeResult(await postJson(endpoints.education, entries));
            if (result.success) {
                tempProfile.Educations = entries;
                persistTempProfile();

                if (advance) {
                    goToStep(3);
                }

                return true;
            }

            showErrors(section, result.errors);
            return false;
        } catch (error) {
            console.error('[Education] AJAX validation error:', error);
            showErrors(section, ['An error occurred validating education.']);
            return false;
        } finally {
            setLoading(button, false);
        }
    }

    async function validateSkillsAndMaybeSubmit(submitAfterValidation) {
        const section = sections[3];
        const button = document.getElementById('submitProfile');

        clearErrors(section);
        setLoading(button, true);

        try {
            const result = normalizeResult(await postForm(endpoints.skills, skillsFormData()));
            if (!result.success) {
                showErrors(section, result.errors);
                return false;
            }

            tempProfile.Skills = collectSkills();
            persistTempProfile();

            if (submitAfterValidation) {
                return await submitProfile();
            }

            return true;
        } catch (error) {
            console.error('[Skills] AJAX validation error:', error);
            showErrors(section, ['An error occurred validating skills.']);
            return false;
        } finally {
            setLoading(button, false);
        }
    }

    async function submitProfile() {
        const section = sections[3];
        const profile = collectFullProfile();

        tempProfile = profile;
        persistTempProfile();

        try {
            const result = normalizeResult(await postJson(endpoints.submit, profile));
            if (result.success) {
                clearTempProfile();
                showSuccess();
                return true;
            }

            showErrors(section, result.errors);
            return false;
        } catch (error) {
            console.error('[Submit] AJAX error:', error);
            showErrors(section, ['An error occurred saving the profile. Please try again.']);
            return false;
        }
    }

    function saveDraft(button) {
        const originalText = button.textContent;

        storeCurrentSection();
        button.disabled = true;
        button.textContent = 'Saved locally';

        setTimeout(() => {
            button.textContent = originalText;
            button.disabled = false;
        }, 1400);
    }

    function showSuccess() {
        if (sections[3]) {
            sections[3].style.display = 'none';
        }

        const success = document.getElementById('success-section');
        if (success) {
            success.style.display = 'block';
            success.scrollIntoView({ behavior: 'smooth', block: 'start' });
        }

        if (progressFill) {
            progressFill.style.width = '100%';
        }

        for (let i = 1; i <= 3; i++) {
            if (dots[i]) {
                dots[i].className = 'progress-dot progress-dot--done';
                dots[i].textContent = '✓';
            }
        }
    }

    let eduIndex = document.querySelectorAll('.education-entry').length - 1;

    function buildEducationEntry(index, data) {
        const div = document.createElement('div');
        div.className = 'education-entry';
        div.dataset.index = index;
        div.dataset.dbId = data?.Id || '';

        div.innerHTML = `
            <div class="entry-header">
                <h4>Education Entry #${index + 1}</h4>
                <button type="button" class="btn btn--danger btn--sm remove-entry">Remove</button>
            </div>
            <div class="form-grid">
                <div class="form-group">
                    <label>Institution Name</label>
                    <input class="form-control" type="text" name="Educations[${index}].Institution" placeholder="Harvard University" value="${escapeAttribute(data?.Institution || '')}" />
                    <span class="field-error"></span>
                </div>
                <div class="form-group">
                    <label>Degree</label>
                    <input class="form-control" type="text" name="Educations[${index}].Degree" placeholder="Bachelor of Science" value="${escapeAttribute(data?.Degree || '')}" />
                    <span class="field-error"></span>
                </div>
                <div class="form-group">
                    <label>Field of Study</label>
                    <input class="form-control" type="text" name="Educations[${index}].FieldOfStudy" placeholder="Computer Science" value="${escapeAttribute(data?.FieldOfStudy || '')}" />
                    <span class="field-error"></span>
                </div>
                <div class="form-group">
                    <label>Graduation Year</label>
                    <input class="form-control" type="number" name="Educations[${index}].GraduationYear" placeholder="2020" min="1950" max="2030" value="${escapeAttribute(data?.GraduationYear || '')}" />
                    <span class="field-error"></span>
                </div>
            </div>
            <div class="form-group">
                <label>Description</label>
                <textarea class="form-control" name="Educations[${index}].Description" rows="2" placeholder="Notable achievements, thesis topic...">${escapeHtml(data?.Description || '')}</textarea>
                <span class="field-error"></span>
            </div>`;

        return div;
    }

    function addEducationEntry() {
        eduIndex += 1;
        document.getElementById('education-entries')?.appendChild(buildEducationEntry(eduIndex));
        updateRemoveButtons();
    }

    function populateEducationEntries(entries) {
        const container = document.getElementById('education-entries');
        if (!container || !Array.isArray(entries) || entries.length === 0) {
            return;
        }

        container.innerHTML = '';
        eduIndex = -1;

        entries.forEach(entry => {
            eduIndex += 1;
            container.appendChild(buildEducationEntry(eduIndex, entry));
        });
    }

    function ensureEducationEntry() {
        if (document.querySelectorAll('#education-entries .education-entry').length === 0) {
            addEducationEntry();
        }
    }

    function updateRemoveButtons() {
        const entries = document.querySelectorAll('#education-entries .education-entry');
        entries.forEach(entry => {
            const remove = entry.querySelector('.remove-entry');
            if (remove) {
                remove.style.display = entries.length > 1 ? 'inline-flex' : 'none';
            }
        });
    }

    function syncSkillsJson() {
        if (skillsJson) {
            skillsJson.value = JSON.stringify(collectSkillEntries());
        }
    }

    let skillIndex = document.querySelectorAll('#skill-entries .skill-entry').length - 1;

    const allSkills = [
        'CSharp', 'Java', 'Python', 'JavaScript', 'SQL', 'HTML', 'CSS',
        'React', 'Angular', 'Vue', 'NodeJS', 'Docker', 'Kubernetes',
        'AWS', 'Azure', 'GCP', 'Git', 'CI_CD', 'AgileMethodologies'
    ];

    function getSelectedSkills(excludeSelect = null) {
        return [...document.querySelectorAll('#skill-entries select[name$=".Name"]')]
            .filter(select => select !== excludeSelect)
            .map(select => select.value)
            .filter(value => value);
    }

    function skillOptions(selected = '', currentSelect = null) {

        const selectedSkills = getSelectedSkills(currentSelect);

        const availableSkills = allSkills.filter(skill =>
            !selectedSkills.includes(skill)
        );

        return ['<option value="">-- Select --</option>']
            .concat(
                availableSkills.map(skill => `
                <option value="${skill}" ${skill === selected ? 'selected' : ''}>
                    ${skill}
                </option>
            `)
            )
            .join('');
    }

    function proficiencyOptions(selected) {
        const levels = ['Beginner', 'Intermediate', 'Advanced', 'Expert'];
        return ['<option value="">-- Select --</option>']
            .concat(levels.map(level => `<option value="${level}" ${level === selected ? 'selected' : ''}>${level}</option>`))
            .join('');
    }

    function buildSkillEntry(index, data) {
        const div = document.createElement('div');
        div.className = 'skill-entry education-entry';
        div.dataset.index = index;
        div.dataset.skillId = data?.SkillId || '';

        div.innerHTML = `
            <div class="entry-header">
                <h4>Skill Entry #${index + 1}</h4>
                <button type="button" class="btn btn--danger btn--sm remove-skill-entry">Remove</button>
            </div>
            <div class="form-grid">
                <div class="form-group">
                    <label>Skill</label>
                    <select class="form-control" name="Skills.SkillEntries[${index}].Name">
                        ${skillOptions(data?.Name || '', null)}
                    </select>
                    <span class="field-error"></span>
                </div>
                <div class="form-group">
                    <label>Proficiency Level</label>
                    <select class="form-control" name="Skills.SkillEntries[${index}].ProficiencyLevel">
                        ${proficiencyOptions(data?.ProficiencyLevel || '')}
                    </select>
                    <span class="field-error"></span>
                </div>
                <div class="form-group">
                    <label>Years of Experience</label>
                    <input class="form-control" type="number" min="0" max="60"
                           name="Skills.SkillEntries[${index}].YearsOfExperience"
                           placeholder="5" value="${escapeAttribute(data?.YearsOfExperience || '')}" />
                    <span class="field-error"></span>
                </div>
            </div>
            <div class="form-group">
                <label>Additional Notes</label>
                <textarea class="form-control" rows="2"
                          name="Skills.SkillEntries[${index}].AdditionalNotes"
                          placeholder="Projects, certifications, frameworks...">${escapeHtml(data?.AdditionalNotes || '')}</textarea>
                <span class="field-error"></span>
            </div>`;

        return div;
    }
    function refreshSkillDropdowns() {

        const selects = document.querySelectorAll(
            '#skill-entries select[name$=".Name"]'
        );

        selects.forEach(select => {

            const currentValue = select.value;

            select.innerHTML = skillOptions(currentValue, select);

            select.value = currentValue;
        });
    }

    function addSkillEntry() {
        if (!skillEntriesContainer) {
            return;
        }

        skillIndex += 1;
        skillEntriesContainer.appendChild(buildSkillEntry(skillIndex));
        updateSkillRemoveButtons();
        refreshSkillDropdowns();
        syncSkillsJson();
    }

    function populateSkillEntries(entries) {
        if (!skillEntriesContainer || !Array.isArray(entries) || entries.length === 0) {
            return;
        }

        skillEntriesContainer.innerHTML = '';
        skillIndex = -1;

        entries.forEach(entry => {
            skillIndex += 1;
            skillEntriesContainer.appendChild(buildSkillEntry(skillIndex, entry));
        });

        updateSkillRemoveButtons();
        syncSkillsJson();
    }

    function ensureSkillEntry() {
        if (document.querySelectorAll('#skill-entries .skill-entry').length === 0) {
            addSkillEntry();
        }
    }

    function updateSkillRemoveButtons() {
        const entries = document.querySelectorAll('#skill-entries .skill-entry');
        entries.forEach(entry => {
            const remove = entry.querySelector('.remove-skill-entry');
            if (remove) {
                remove.style.display = entries.length > 1 ? 'inline-flex' : 'none';
            }
        });
    }

    function escapeHtml(value) {
        return String(value)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }

    function escapeAttribute(value) {
        return escapeHtml(value);
    }

    function restoreTempProfile() {
        if (!tempProfile || !sessionStorage.getItem(draftKey)) {
            syncSkillsJson();
            return;
        }

        const personal = tempProfile.PersonalInfo || {};
        setValue('#PersonalInfo_FirstName', personal.FirstName);
        setValue('#PersonalInfo_LastName', personal.LastName);
        setValue('#PersonalInfo_Phone', personal.Phone);
        setValue('#PersonalInfo_Location', personal.Location);
        setValue('#PersonalInfo_DateOfBirth', personal.DateOfBirth);
        setValue('#PersonalInfo_Headline', personal.Headline);
        setValue('#PersonalInfo_Bio', personal.Bio);
        setValue('#ProfilePictureUrlValue', personal.ProfilePictureUrl);

        populateEducationEntries(tempProfile.Educations);

        if (tempProfile.Skills?.SkillEntries) {
            populateSkillEntries(tempProfile.Skills.SkillEntries);
        } else if (tempProfile.Skills?.SkillNames) {
            populateSkillEntries(tempProfile.Skills.SkillNames.map(name => ({ Name: name })));
        }
    }

    function wirePhotoUpload() {
        const uploadButton = document.getElementById('uploadPhotoBtn');
        const input = document.getElementById('PhotoUpload');

        uploadButton?.addEventListener('click', () => input?.click());
        input?.addEventListener('change', function () {
            const file = this.files?.[0];
            if (!file) {
                return;
            }

            const reader = new FileReader();
            reader.onload = event => {
                const preview = document.getElementById('avatar-preview');
                if (preview) {
                    preview.innerHTML = `<img src="${event.target.result}" alt="Profile Photo" />`;
                }
            };
            reader.readAsDataURL(file);

            const formData = new FormData();
            formData.append('photo', file);

            postForm(endpoints.photo, formData)
                .then(result => {
                    const normalized = normalizeResult(result);
                    if (normalized.success && result.url) {
                        setValue('#ProfilePictureUrlValue', result.url);
                        tempProfile.PersonalInfo = collectPersonalInfo();
                        persistTempProfile();
                    }
                })
                .catch(error => console.error('[Photo] AJAX error:', error));
        });
    }

    document.getElementById('nextToSection2')?.addEventListener('click', () => validatePersonalInfo(true));
    document.getElementById('nextToSection3')?.addEventListener('click', () => validateEducation(true));
    document.getElementById('submitProfile')?.addEventListener('click', () => validateSkillsAndMaybeSubmit(true));
    document.getElementById('backToSection1')?.addEventListener('click', () => goToStep(1));
    document.getElementById('backToSection2')?.addEventListener('click', () => goToStep(2));
    document.getElementById('saveDraft1')?.addEventListener('click', function () { saveDraft(this); });
    document.getElementById('saveDraft2')?.addEventListener('click', function () { saveDraft(this); });
    document.getElementById('saveDraft3')?.addEventListener('click', function () { saveDraft(this); });
    document.getElementById('addEducation')?.addEventListener('click', addEducationEntry);
    document.getElementById('education-entries')?.addEventListener('click', event => {
        if (event.target.classList.contains('remove-entry')) {
            event.target.closest('.education-entry')?.remove();
            ensureEducationEntry();
            updateRemoveButtons();
        }
    });

    document.getElementById('addSkill')?.addEventListener('click', addSkillEntry);
    skillEntriesContainer?.addEventListener('click', event => {
        if (event.target.classList.contains('remove-skill-entry')) {
            event.target.closest('.skill-entry')?.remove();
            ensureSkillEntry();
            updateSkillRemoveButtons();
            refreshSkillDropdowns();
            syncSkillsJson();
        }
    });
    skillEntriesContainer?.addEventListener('change', event => {

        if (event.target.matches('select[name$=".Name"]')) {

            refreshSkillDropdowns();

            syncSkillsJson();
        }
    });
    restoreTempProfile();
    ensureEducationEntry();
    ensureSkillEntry();
    updateRemoveButtons();
    updateSkillRemoveButtons();
    syncSkillsJson();
    wirePhotoUpload();
    goToStep(1);
}());
