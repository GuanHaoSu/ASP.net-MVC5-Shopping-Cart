var ValidMode = function () {
    //var idArray =[];
    return {
        PatternArray: [],
        idArray: [],
        ClassName: "",
        requireText: "",
        textStytle: "",
        checkImgUrl: "",
        valid_Init: function (ClassName, requireText, textStytle, checkImgUrl) {
            this.requireText = requireText;
            this.textStytle = textStytle;
            this.checkImgUrl = checkImgUrl;
            this.ClassName = ClassName;
            if (checkImgUrl == undefined) {
                this.checkImgUrl = "";
            }
            else {
                this.checkImgUrl = checkImgUrl;
            }
            if (this.ClassName != "") {
                for (var i = 0; i < $('.' + this.ClassName).length; i++) {
                    //this.idArray.push($('.' + this.ClassName)[i].id);
                    this.AddId($('.' + this.ClassName)[i].id);
                }
            }
            else {
                alert('Please Init ClassName');
            }
        },
        runValid: function () {
            var pass = true;
            var finalPass = true;
            var i = 0;
            for (i = 0; i < this.idArray.length; i++) {
                $('#' + this.idArray[i].id + 'lbl').remove();
                var ids = $('#' + this.idArray[i].id).val();
                if (ids == "") {
                    $('#' + this.idArray[i].id).after("<label id='" + this.idArray[i].id + "lbl' style='" + this.textStytle + "'>" + this.requireText + "</label>");
                    pass = false;
                }
            }
            if (this.PatternArray.length != 0) {
                var addtionPass = this.addtionVaild();
                if (pass != true || addtionPass != true) {
                    finalPass = false;
                }
            }
            else if (this.PatternArray.length == 0) {
                if (pass != true) {
                    finalPass = false;
                }
            }
            return finalPass;
        },
        validPattern: function (PatternName, Regex, Alert) {
            this.PatternName = PatternName;
            var i = 0;
            var passPattern = true;
            for (i = 0; i < this.idArray.length; i++) {
                var ids = $('#' + this.idArray[i].id).val();
                if (this.idArray[i].id == PatternName && ids != "") {
                    var validPart = $('#' + this.idArray[i].id).val().search(Regex);
                    if (validPart == -1) {
                        $('#' + this.idArray[i].id + 'lbl').remove();
                        $('#' + this.idArray[i].id).after("<label id='" + this.idArray[i].id + "lbl' style='" + this.textStytle + "'>" + Alert + "</label>");
                        passPattern = false;
                    }
                    else {
                        $('#' + this.idArray[i].id + 'lbl').remove();
                    }
                }
            }
            return passPattern;

        },
        KeyPressInit: function () {
            var i = 0;
            if (this.idArray.length > 0) {
                for (var j = 0; j < this.idArray.length; j++) {
                    $('#' + this.idArray[j].id + '.' + this.ClassName).bind('keyup', this.idArray,
                   function (event) {
                       var Array = event.data;
                       var text = event.currentTarget.value;
                       var id = event.currentTarget.id;
                       if (text != "") {
                           for (var x = 0; x < Array.length; x++) {
                               if (Array[x].id == id) {
                                   $('#' + Array[x].id + 'lbl').remove();
                                   $('#' + Array[x].id + 'check').remove();
                                   if (Array[x].checkImgUrl != "") {
                                       $('#' + Array[x].id).after("<img id='" + Array[x].id + "check' src='" + Array[x].checkImgUrl + "'>");
                                   }
                               }
                           }
                       }
                       if (text == "") {
                           for (var x = 0; x < Array.length; x++) {
                               if (Array[x].id == id) {
                                   $('#' + Array[x].id + 'lbl').remove();
                                   $('#' + Array[x].id + 'check').remove();
                                   $('#' + Array[x].id).after("<label id='" + Array[x].id + "lbl' style='" + Array[x].textStytle + "'>" + Array[x].requireText + "</label>");
                               }
                           }
                       }

                   });
                }
            }
            if (this.PatternArray.length != 0) {
                for (i = 0; i < this.PatternArray.length; i++) {
                    $('#' + this.PatternArray[i].PatternName + '.' + this.ClassName).bind('keyup', this.PatternArray,
                        function (event) {
                            var Array = event.data;
                            var text = event.currentTarget.value;
                            var id = event.currentTarget.id;
                            for (var x = 0; x < Array.length; x++) {
                                if (Array[x].PatternName == id) {
                                    var validPart = text.search(Array[x].Regex);
                                    if (validPart == -1) {
                                        $('#' + Array[x].PatternName + 'lbl').remove();
                                        $('#' + Array[x].PatternName + 'check').remove();
                                        $('#' + Array[x].PatternName).after("<label id='" + Array[x].PatternName + "lbl' style='" + Array[x].textStytle + "'>" + Array[x].Alert + "</label>");

                                    }
                                    else {
                                        $('#' + Array[x].PatternName + 'lbl').remove();
                                        $('#' + Array[x].PatternName + 'check').remove();
                                        if (Array[x].checkImgUrl != "") {
                                            $('#' + Array[x].PatternName).after("<img id='" + Array[x].PatternName + "check' src='" + Array[x].checkImgUrl + "'>");
                                        }
                                    }

                                }
                            }

                        });

                }
            }

        },
        addtionVaild: function () {
            var i = 0;
            var pass = true;
            if (this.PatternArray.length != 0) {
                for (i = 0; i < this.PatternArray.length; i++) {

                    var PassPattern = this.validPattern(this.PatternArray[i].PatternName, this.PatternArray[i].Regex, this.PatternArray[i].Alert);
                    if (PassPattern != true) {
                        pass = false;
                    }
                }
            }
            return pass;
        },
        AddId: function (ID) {
            var Id_Obj = {
                id: ID,
                textStytle: this.textStytle,
                requireText: this.requireText,
                checkImgUrl: this.checkImgUrl
            };
            this.idArray.push(Id_Obj);
        },
        AddRegexPatern: function (patternName, regex, alert) {
            var PatternObj = {
                PatternName: patternName,
                Regex: regex,
                Alert: alert,
                textStytle: this.textStytle,
                checkImgUrl: this.checkImgUrl
            };
            this.PatternArray.push(PatternObj);
        },
        ClearValid: function () {
            for (i = 0; i < this.idArray.length; i++) {
                $('#' + this.idArray[i].id + 'lbl').remove();
                $('#' + this.idArray[i].id + 'check').remove();
            }

        }

    };
};