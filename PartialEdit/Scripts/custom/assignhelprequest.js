$(function () {
    $('.dialogLink').on('click', function () {
        var element = $(this);

        // Retrieve values from the HTML5 data attributes of the link        
        var dialogTitle = element.attr('data-dialog-title');
        var updateTargetId = '#' + element.attr('data-update-target-id');
        var updateUrl = element.attr('data-update-url');

        // Generate a unique id for the dialog div
        var dialogId = 'assign-dialog-' + Math.floor(Math.random() * 1000);
        var dialogDiv = "<div id='" + dialogId + "'></div>";
        var idList = "";
        var ids = [];
        $("#organisation-match-container input[type=checkbox]:checked").each(function () {
            var row = $(this).closest("tr")[0];
            idList = row.cells[1].innerText;
            ids.push(idList);
            idList = "";
        });

        $(dialogDiv).load(this.href, function () {
            //var selectedValue = document.getElementById("AssignedTo");
            //console.log(selectedValue);
            var data = JSON.stringify({ 'ids': ids, 'assignedTo': 'Jian.Song@dese.gov.au' });
            $(this).dialog({
                modal: true,
                resizable: false,
                title: dialogTitle,
                create: function (event, ui) {
                    console.log('Created');
                },
                open: function (event, ui) {
                    console.log('Opened');
                },
                buttons: {
                    "Save": function () {
                        const checks = $('.isEdit:checkbox:checked');
                        let ids = [];
                        for (var i = 0; i < checks.length; i++) {
                            const dt = checks[i].id;
                            ids.push(dt);
                        }
                        const id_s = ids.join(",");

                        var input = $("<input>")
                            .attr("type", "hidden")
                            .attr("name", "selectedids").val(id_s);
                        $('form').append(input);

                        var form = $('form', this);
                        $(form).submit();
                    },
                    //"SaveBulk": {
                    //    text: "BulkSave",
                    //    id: "Update",
                    //    click: function () {
                    //        const checks = $('.isEdit:checkbox:checked');
                    //        let ids = [];
                    //        for (var i = 0; i < checks.length; i++) {
                    //            const dt = checks[i].id;
                    //            ids.push(dt);
                    //        }

                    //        const s = $('#AssignedTo :selected').text();
                    //        let data = {
                    //            Ids: ids,
                    //            AssinedTo: s
                    //        };

                    //        const id_s = ids.join(",");
                    //        console.log(data);

                    //        var input = $("<input>")
                    //            .attr("type", "hidden")
                    //            .attr("name", "selectedids").val(id_s);
                    //        $('form').append(input);

                    //        var form = $('form', this);
                    //        console.log(form);
                    //        $(form).submit();

                    //        //$.ajax({
                    //        //    type: "POST",
                    //        //    url: "https://secure.localhost/admin/AssignHelpRequest/BulkAssign",
                    //        //    data: data,
                    //        //    contentType: "application/json; charset=utf-8",
                    //        //    dataType: "json",
                    //        //    success: function (r) {
                    //        //        alert(r + " record(s) inserted.");
                    //        //    }
                    //        //});
                    //    }
                    //},
                    "Cancel": function () {
                        $(this).dialog('close');
                        element.focus();
                    }
                }
            });

            // Enable client side validation
            // $.validator.unobtrusive.parse(this);

            // Setup the ajax submit logic
            //  wireUpForm(this, updateTargetId, updateUrl);
        });
        return false;
    });
});

function wireUpForm(dialog, updateTargetId, updateUrl) {
    $('form', dialog).submit(function () {

        // Do not submit if the form
        // does not pass client side validation
        if (!$(this).valid())
            return false;

        // Client side validation passed, submit the form
        // using the jQuery.ajax form
        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            success: function (result) {
                // Check whether the post was successful
                if (result.success) {
                    location.reload();
                } else {
                    // Reload the dialog to show model errors                    
                    $(dialog).html(result);

                    // Enable client side validation
                    $.validator.unobtrusive.parse(dialog);

                    // Setup the ajax submit logic
                    wireUpForm(dialog, updateTargetId, updateUrl);
                }
            }
        });
        return false;
    });
}