<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="TODOweb.Test" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div id="page-wrap">
        <!-- Header Section -->
        <div id="header">
            <h1><a href="#">PHP Sample Test App</a></h1>
        </div>

        <!-- Main Section -->
        <div id="main" class="myMain">

            <!-- Todo List -->
            <ul id="list" class="ui-sortable">
                <asp:Repeater ID="Repeater1" runat="server">
                    <ItemTemplate>
                        <li class="draggable-row" 
                            id="row_<%# Eval("ListItemId") %>" 
                            data-item-id='<%# Eval("ListItemId") %>' 
                            data-is-done='<%# Eval("IsDone") != null && Convert.ToBoolean(Eval("IsDone")) ? "true" : "false" %>'>
                            
                            <!-- Item Description -->                          
                             <span id="<%# Eval("ListItemId") %>listitem"  
                                 style='<%# "background-color: " + Eval("ListColor") + "; color: white;" %>'
                                 ondblclick="editDescription('<%# Eval("ListItemId") %>', '<%# Eval("Description") %>' , '<%#Eval("IsDone") != null && Convert.ToBoolean(Eval("IsDone"))? "true" : "false"  %>')">
                                 <%# Eval("Description") %>

</span>


                            <!-- Dragger Tab -->
                            <div class="draggertab tab button drag-handle">
                        
                            </div>                          

                            <div class="colortab tab" data-row-id="<%# Eval("ListItemId") %>">
                           <input type="color" class="color-picker" id="colorPicker_<%# Eval("ListItemId") %>" 
                              value='<%# Eval("ListColor") %>' />
                          </div>

                             <div class="deletetab tab button delete" onclick="deleteRecord('<%# Eval("ListItemId") %>')">
           
        </div>
                           <!-- Done Tab -->
                            <div class="donetab tab button done-button ">
                               
                            </div>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
        </div>

        <!-- Add New Item -->
        <div class="text-box mt-5">

            <div class="box-parent" style="display: flex; justify-content: center;">
                <asp:TextBox ID="add_new" runat="server" placeholder="Enter Description" CssClass="input-field"></asp:TextBox>
                 <asp:Button ID="btnSave" runat="server" Text="Add" OnClick="btnSave_Click"  
                  EnableViewState="true"  class="button" CssClass="myBtn" />
                 <asp:HiddenField ID="hdnItemId" runat="server" Value="0" />
                 <asp:HiddenField ID="hdnMode" runat="server" Value="Add" />

            </div>
               <span id="errorSpan" style="color: red; text-align : center; display: flex; justify-content: center; margin: 1rem 18rem;"></span>

        </div>
    </div>

    <!-- JavaScript Dependencies -->
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://code.jquery.com/ui/1.13.2/jquery-ui.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/spectrum/1.8.1/spectrum.min.js"></script>


    <script>

        //function confirmAction() {
        //    confirm("Are you sure to update this record?");
        //}


        function editDescription(id, description, isDone) {
            if (isDone === "true") {
                alert("This item is already marked as done and cannot be edited.");
                document.getElementById('<%= add_new.ClientID %>').disabled = true; // Disable textbox
                document.getElementById('<%= btnSave.ClientID %>').disabled = true; // Disable button
                window.location.reload();
                return;
            } else {
                document.getElementById('<%= hdnItemId.ClientID %>').value = id;
                document.getElementById('<%= add_new.ClientID %>').value = description; // Textbox
                document.getElementById('<%= btnSave.ClientID %>').value = "Update";
                document.getElementById('<%= hdnMode.ClientID %>').value = "Update"; // Set mode to Update


            }
          

        }

        function deleteRecord(itemId) {
            if (confirm("Are you sure you want to delete this record?")) {
                __doPostBack('DeleteRecord', itemId);
            }
        }

        $(document).ready(function () {
            // Handle 'Done' button clicks
            $(".done-button").click(function () {
                var itemId = $(this).closest("li").data("item-id");
                
                // Trigger the __doPostBack to notify the server
                __doPostBack("MarkAsDone", itemId);
            });

            // Apply styles to items marked as done
            applyDoneStyles();
        });

        function applyDoneStyles() {
            $("li[data-item-id]").each(function () {
                var row = $(this);
                var isDone = row.data("is-done");

                // Apply styles if the item is marked as done
                if (isDone === true || isDone === "true") {
                    row.css({
                        "text-decoration": "line-through",
                        "opacity": "0.6"
                    });
                }
            });
        }

        $(document).ready(function () {
            // Enable sorting on the ordered list
            $("#list").sortable({
                items: ".draggable-row",
                handle: ".drag-handle",
                update: function () {
                    // Collect new order of ListItemIds
                    let idsInOrder = [];
                    $(".draggable-row").each(function () {
                        idsInOrder.push($(this).data("item-id"));
                    });
                    // Send updated order to the server using __doPostBack
                    __doPostBack("UpdatePositions", JSON.stringify(idsInOrder));
                }
            });
        });

        $(document).ready(function () {
            // Initialize the color picker for each list item
            $(".color-picker").each(function () {
                const pickerId = $(this).attr("id"); // Unique ID for the picker
                const rowId = pickerId.split("_")[1]; // Extract the row ID

                // Set a default color if none is provided
                if (!$(this).val() || $(this).val() === "#000000") {
                    $(this).val("#ff0000");
                }

                // Initialize the spectrum color picker
                $("#" + pickerId).spectrum({
                    color: $(this).val() || "#ff0000", // Default color
                    showPalette: true,
                    palette: [
                        ["#ff0000", "#ffffff", "#000000", "#00ff00", "#0000ff"],
                        ["#ffff00", "#ff00ff", "#00ffff", "#808080", "#ffa500"]
                    ],
                    change: function (color) {
                        // Update the colortab background color
                        $(`.colortab[data-row-id='${rowId}']`).css("background-color", color.toHexString());

                        // Optionally, send the color update to the server
                        __doPostBack("UpdateRowColor", `${rowId};${color.toHexString()}`);
                    }
                });

                // Initially hide the spectrum UI
                $("#" + pickerId).spectrum("hide");
            });

            // Show or hide the color picker on button click
            $(".colortab").on("click", function (e) {
                e.stopPropagation(); // Prevent event bubbling
                const rowId = $(this).data("row-id");
                const pickerId = `#colorPicker_${rowId}`;

                // Get the position of the colortab
                const colortabOffset = $(this).offset();
                const colorPickerContainer = $(pickerId).spectrum("container");

                // Check if the container exists and adjust its position
                if (colorPickerContainer.length) {
                    colorPickerContainer.css({
                        top: colortabOffset.top + $(this).outerHeight() + 5, // Position below the colortab
                        left: colortabOffset.left, // Align it to the left of the colortab
                        position: 'absolute' // Make sure the container is absolutely positioned
                    });
                }

                // Toggle visibility of the color picker
                if (colorPickerContainer.is(":visible")) {
                    $(pickerId).spectrum("hide");
                } else {
                    $(".color-picker").spectrum("hide"); // Hide all other pickers
                    $(pickerId).spectrum("show"); // Show the selected picker
                }
            });

            // Hide spectrum on outside click
            $(document).on("click", function (e) {
                if (!$(e.target).closest(".colortab, .sp-container").length) {
                    $(".color-picker").spectrum("hide");
                    // Ensure sp-replacer is hidden
                    $('.sp-replacer').css('display', 'none');
                }
            });
        });





     


    </script>





</asp:Content>
