﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MeasureDashboardCampus.ascx.cs" Inherits="RockWeb.Plugins.church_ccv.Steps.MeasureDashboardCampus" %>

<style>
    .measure {
        display: flex;
    }

    h2 small {
        font-size: 14px;
    }

    .measure h4 {
        margin-bottom: 4px;
    }
    
    .measure-value{
        padding-top: 12px;
    }

    .measure-icon {
        width: 60px;
        float: left;
        font-size: 40px;
        padding-top: 31px;
    }

    .measure-details {
        width: 100%;
    }
    
    .measure-bar {
        height: 40px;
        position: relative;
    }

    .is-tbd .measure-bar,
    .is-tbd .measure-value {
        display: none;
    }

    .measure-tbd {
        font-size: 20px;
        opacity: .5;
        padding-top: 6px;
    }

    .measure-bar .progress-bar{
        font-size: 18px;
        padding-top: 9px;
    }

    .progress-compare {
        border-right: 1px solid <%=_compareColor %>;
        height: 40px;
        position: absolute;
    }
</style>

<script>

    Sys.Application.add_load( function () {
        $('.value-tip').tooltip();

        $(".js-settings-toggle").on("click", function () {
            $('.js-settings-panel').slideToggle();
        });
    });

</script>

<asp:UpdatePanel ID="upnlContent" runat="server">
    <ContentTemplate>

        <asp:Panel ID="pnlView" runat="server" CssClass="panel panel-block">
        
            <div class="panel-heading">
                <h1 class="panel-title"><i class="fa fa-bar-chart"></i> Measure Dashboard</h1>
                <div class="btn-group btn-toggle margin-h-sm">
                    <Rock:BootstrapButton ID="bsAdults" runat="server" CssClass="btn btn-default btn-xs" AccessKey="s" OnClick="bbtnView_Click" Text="Adults" />
                    <Rock:BootstrapButton ID="bsStudents" runat="server" CssClass="btn btn-default btn-xs" AccessKey="s" OnClick="bbtnView_Click" Text="Students" />
                    <Rock:BootstrapButton ID="bsTotal" runat="server" CssClass="btn btn-default btn-xs" AccessKey="s" OnClick="bbtnView_Click" Text="Total" />
                </div>
                <div class="pull-right">

                    <asp:Panel ID="pnlStatus" runat="server">
                        <div class="toggle-container">
                            <Rock:BootstrapButton ID="bsWeekendAttendance" runat="server" CssClass="btn btn-default btn-xs" AccessKey="s" OnClick="bbtnView_Click" Text="Weekend Attendance" />                           
                            <Rock:HighlightLabel ID="hlDate" CssClass="js-settings-toggle cursor-pointer" runat="server" />
                        </div>
                    </asp:Panel>
                    
                    
                </div>
            </div>
            <div class="panel-body">
                <div class="panel-settings js-settings-panel" style="display: none;">
                    <div class="row">
                        <div class="col-md-4">
                            <Rock:RockDropDownList ID="ddlSundayDates" Label="Recent Dates" runat="server" Help="The last 12 Sundays." />
                        </div>
                        <div class="col-md-4">
                            <Rock:DatePicker ID="dpSundayPicker" Label="Specific Date" runat="server" Help="Select a specific date you would like to report on. The system will select the Sunday Date for the date you select." />
                        </div>
                        <div class="col-md-4 text-right">
                            <asp:LinkButton ID="lbSetDate" runat="server" CssClass="btn btn-primary btn-sm margin-t-lg" Text="Set Date" OnClick="lbSetDate_Click" />
                        </div>
                    </div>
                </div>

                <asp:Panel ID="pnlCampus" runat="server">
                    <div class="row">
                        <div class="col-md-8">
                            <h2><asp:Literal ID="lCampusTitle" runat="server" /></h2>
                            
                            <h5>Percentages are of <asp:Literal ID="lMeasureCompareValueSum" runat="server" />
                            <% if( DashboardViewState == DashboardView.WeekendAttendance || DashboardViewState == DashboardView.Total ) %>
                            <% { %>
                                     Adults and Students
                            <% } %>
                            <% else %>
                            <% { %>
                                    <%=DashboardViewState == DashboardView.Students ? "Students" : "Adults"%>
                            <% } %>
                            (Must be Active and Member or Attendee) 
                            </h5>
                        </div>
                        <div class="col-md-4">
                            <Rock:CampusPicker ID="cpCampus" runat="server" OnSelectedIndexChanged="cpCampus_SelectedIndexChanged" AutoPostBack="true" />
                        </div>
                    </div>

                    <Rock:NotificationBox ID="nbMessages" runat="server" />

                    <div class="row">
                        <asp:Repeater ID="rptCampusMeasures" runat="server">
                            <ItemTemplate>
                                <a href="?MeasureId=<%# Eval("MeasureId") %>&CompareTo=<%=DashboardViewState != DashboardView.WeekendAttendance%><%# MeasureDate != null ? "&Date=" + MeasureDate.Value.ToShortDateString() : "" %><%# "&DashboardView=" + DashboardViewState.ToString( ) %>">
                                    <div class="col-md-6 measure">
                                        <div class="measure-icon hidden-sm hidden-xs">
                                            <i class="fa fa-fw <%# Eval("IconCssClass") %>" style="color: <%# Eval("MeasureColor") %>;"></i>
                                        </div>

                                        <div class="measure-details <%# (bool)Eval("IsTbd") ? "is-tbd" : ""%>">
                                    
                                            <div class="clearfix">
                                                <div class="pull-left value-tip" data-toggle="tooltip" data-placement="top" title="<%# Eval("Description") %>">
                                                    <h4 class="pull-left"><%# Eval("Title") %></h4>
                                                </div>
                                                <div class="pull-right measure-value">
                                                    <div class="value-tip" data-toggle="tooltip" data-placement="top" title="<%# Eval("MeasureValue","{0:#,0}") %> individuals have taken this step"><%# Eval("MeasureValue","{0:#,0}") %></div>
                                                </div>
                                            </div>
                                            <%# (bool)Eval("IsTbd") ? "<div class='measure-tbd'>TBD</div>" : ""%>
                                            <div class="progress measure-bar <%# (int)Eval("Percentage") > 100 ? "percent-over-100": "" %>" style="background-color: <%# Eval("MeasureColorBackground") %>; ">
                                                  <div class="progress-compare" style="width: <%# Eval("HistoricalPercentage") %>%; <%# (int)Eval("HistoricalPercentage") == 0 ? "display: none;": "" %>" ></div>
                                                  <div class="progress-bar" role="progressbar" aria-valuenow="60" aria-valuemin="0" aria-valuemax="100" style="width: <%# Eval("Percentage") %>%; background-color: <%# Eval("MeasureColor") %>;">
                                                    <%# Eval("Percentage") %>%
                                                  </div>
                                            </div>
                                        </div>
                                    </div>
                                </a>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </asp:Panel>
                
                <asp:Panel ID="pnlMeasure" runat="server">
                    <h2><asp:Literal ID="lMeasureIcon" runat="server" /> <asp:Literal id="lMeasureTitle" runat="server" /></h2>
                    <p>
                        <asp:Literal ID="lMeasureDescription" runat="server" />
                    </p>

                    <!-- Campus Chart -->
                    <div class="row">
                        <div class="col-md-6 measure ">
                            <div class="measure-details">
                                <div class="clearfix">
                                    <h4 class="pull-left">All Campuses</h4>
                                </div>

                                <div class="clearfix">
                                    <div class="pull-left">
                                        <p>Viewing 
                                        <% if( DashboardViewState == DashboardView.WeekendAttendance || DashboardViewState == DashboardView.Total ) %>
                                        <% { %>
                                                    Adults and Students
                                        <% } %>
                                        <% else %>
                                        <% { %>
                                                <%=DashboardViewState == DashboardView.Students ? "Students" : "Adults"%>
                                        <% } %>
                                        (Must be Active and Member or Attendee) 
                                        </p>
                                    </div>

                                    <div class="pull-right">
                                        <asp:Literal ID="lMeasureCampusSumValue" runat="server" />
                                    </div>
                                </div>
                                
                                <div class="progress measure-bar <%# (int)Eval("Percentage") > 100 ? "percent-over-100": "" %>" style='background-color: <asp:Literal id="lMeasureBackgroundColor" runat="server" />'>
                                    <div class="progress-compare" style="width: <asp:Literal id="lMeasureBarHistoricalPercent" runat="server" />%" ></div>
                                    <div class="progress-bar" role="progressbar" aria-valuenow="60" aria-valuemin="0" aria-valuemax="100" style='width: <asp:Literal id="lMeasureBarPercent" runat="server" />%; background-color: <asp:Literal id="lMeasureColor" runat="server" />;'>
                                        <asp:Literal id="lMeasureBarTextPercent" runat="server" />%
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <hr />

                    <asp:Repeater ID="rptMeasuresByCampus" runat="server">
                        <ItemTemplate>
                                <div class="row">
                                    <div class="col-md-6 measure">
                                        <div class="measure-details">
                                            <div class="clearfix">
                                                <h4 class="pull-left"><%# Eval("Campus") %></h4>
                                            </div>

                                            <div class="clearfix">
                                                <div class="pull-left">
                                                    <p>Viewing </b>
                                                    <% if( DashboardViewState == DashboardView.WeekendAttendance || DashboardViewState == DashboardView.Total ) %>
                                                    <% { %>
                                                             Adults and Students
                                                    <% } %>
                                                    <% else %>
                                                    <% { %>
                                                            <%=DashboardViewState == DashboardView.Students ? "Students" : "Adults"%>
                                                    <% } %>
                                                    (Must be Active and Member or Attendee) 
                                                    </p>
                                                </div>

                                                <div class="pull-right">
                                                    <div class="value-tip" data-toggle="tooltip" data-placement="top" title="<%# Eval("MeasureValue","{0:#,0}") %> individuals have taken this step"><%# Eval("MeasureValue","{0:#,0}") %> / <%# Eval("MeasureCompareValue", "{0:#,0}" ) %></div>
                                                </div>
                                            </div>

                                            <div class="progress measure-bar <%# (int)Eval("Percentage") > 100 ? "percent-over-100": "" %>" style="background-color: <%# Eval("MeasureColorBackground") %>; ">
                                                  <div class="progress-compare" style="width: <%# Eval("HistoricalPercentage") %>%; <%# Eval("HistoricalPercentage") %>%; <%# (int)Eval("HistoricalPercentage") == 0 ? "display: none;": "" %>" ></div>
                                                  <div class="progress-bar" role="progressbar" aria-valuenow="60" aria-valuemin="0" aria-valuemax="100" style="width: <%# Eval("Percentage") %>%; background-color: <%# Eval("MeasureColor") %>;">
                                                    <%# Eval("Percentage") %>%
                                                  </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                    </asp:Repeater>

                    <asp:LinkButton ID="btnBackToCampus" runat="server" CssClass="btn btn-default" OnClick="btnBackToCampus_Click"><i class="fa fa-chevron-left"></i> Campus View</asp:LinkButton>
                </asp:Panel>

                <asp:Literal ID="lComparisonLegend" runat="server" />

                <div class="well text-center margin-t-md">
                    <asp:Literal ID="lOverhaulDate" runat="server">Metric logic significantly updated on 10/9/2016</asp:Literal>
                </div>

            </div>
        
        </asp:Panel>

    </ContentTemplate>
</asp:UpdatePanel>
