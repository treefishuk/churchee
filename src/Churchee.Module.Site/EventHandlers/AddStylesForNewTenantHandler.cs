using Churchee.Common.Storage;
using Churchee.CQRS.Abstractions;
using Churchee.Module.Site.Entities;
using Churchee.Module.Tenancy.Events;

namespace Churchee.Module.Site.EventHandlers
{
    public class AddStylesForNewTenantHandler : INotificationHandler<TenantAddedEvent>
    {

        private readonly IDataStore _dataStore;

        public AddStylesForNewTenantHandler(IDataStore store)
        {
            _dataStore = store;
        }

        public async Task Handle(TenantAddedEvent notification, CancellationToken cancellationToken)
        {
            var applicationTenantId = notification.ApplicationTenantId;

            var repo = _dataStore.GetRepository<Css>();

            var css = new Css(applicationTenantId);

            css.SetStyles(Styles);

            repo.Create(css);

            await _dataStore.SaveChangesAsync(cancellationToken);
        }

        private string Styles => """"

            // ------ Variable Overrides 
            $primary: #009898;   
            $secondary: #fe9700;
            $tertiary: #7f1a7e;
            $link-color: #3a4a63;
            $border-radius: 1rem;
            $btn-border-radius: 0.3rem;
            $link-decoration: none;
            $font-size-root: 18px;
            $line-height-base: 1.6;
            $body-bg: #fff;
            $navbar-bg: #ffffff;
            $headings-font-family: system-ui, -apple-system, "Segoe UI", sans-serif;
            $font-family-sans-serif: system-ui, -apple-system, "Segoe UI", Roboto, "Helvetica Neue", "Noto Sans", "Liberation Sans", Arial, sans-serif, "Apple Color Emoji", "Segoe UI Emoji", "Segoe UI Symbol", "Noto Color Emoji";
            $card-bg: #fff;
            $card-color: #555;
            @import "functions";
            @import "variables";
            @import "variables-dark";
            @import "maps";
            @import "mixins";
            @import "utilities";
            @import "root";
            @import "reboot";
            @import "type";
            $modular-ratio: 1.25;
            $spacer: 4px; // base spacing unit
            
            // Mixin for modular heading styles

            $heading-font-size-root: 14px;

            @mixin modular-heading($level, $steps) {
              $font-size: $heading-font-size-root * pow($modular-ratio, $steps);
              $line-height: 1.3; // tighter for headings
              $margin-top: $spacer * ($steps + 3); // more space above
              $margin-bottom: $spacer * ($steps + 1); // less space below

              h#{$level} {
                font-size: $font-size;
                line-height: $line-height;
                margin-top: $margin-top;
                margin-bottom: $margin-bottom;
              }
            }

     
            // Apply mixin to headings
            @include modular-heading(1, 5);
            @include modular-heading(2, 4);
            @include modular-heading(3, 3);
            @include modular-heading(4, 2);
            @include modular-heading(5, 1);
            @include modular-heading(6, 0);
            @import "images";
            @import "containers";
            @import "grid";
            @import "tables";
            @import "forms";

       
            // Buttons ----------------------------------------------------------------------------------

            @import "buttons";

            .btn {
              padding-bottom: 0.1rem;
            }

            .btn-white-outline {
              color: #fff;
              border: 1px solid #ffffff;
              background-color: transparent;

              &:hover {
                background-color: #ffffff;
                color: $secondary;
              }
            }

            .btn-dark {
              @include button-variant(
                #383838,
                #383838,
                #fff
              );
            }

            .btn-primary, .btn-secondary {
                color: #fff;

                &:hover{
                    color: #fff;
                }
            }


            @import "transitions";
            @import "dropdown";
            @import "button-group";
            @import "nav";
            @import "navbar";

            .navbar{
                background: $navbar-bg;
                transition: transform 0.25s ease;

            }

            .navbar-brand{
                img{
                    height: 3rem;
                }
            }

            .navbar.hide {
              transform: translateY(-100%);
            }
            .navbar-toggler, .navbar-toggler:focus {
                border: none;
                box-shadow: none;
                height: 33px;
                width: 40px;
                padding: 0;
                margin: 0 0.5rem 0 0;
                position: relative;
            }


            .navbar-toggler span {
                display: block;
                height: 5px;
                width: 100%;
                background: #770b73;
                border-radius: 9px;
                -webkit-transition: .25s ease-in-out;
                -moz-transition: .25s ease-in-out;
                -o-transition: .25s ease-in-out;
                transition: .25s ease-in-out;
                position: absolute;
                margin-top: -4px;
            }

            .navbar-toggler.collapsed span:nth-child(1) {
              top: 5px;
            }

            .navbar-toggler.collapsed span:nth-child(2),.navbar-toggler.collapsed span:nth-child(3) {
              top: 18px;
            }

            .navbar-toggler.collapsed span:nth-child(4) {
              top: 31px;
            }



            .navbar-toggler span:nth-child(2) {
              -webkit-transform: rotate(45deg);
              -moz-transform: rotate(45deg);
              -o-transform: rotate(45deg);
              transform: rotate(45deg);
            }

            .navbar-toggler.collapsed:nth-child(2) span, .navbar-toggler.collapsed:nth-child(3) span {
                -webkit-transform: rotate(0deg);
                -moz-transform: rotate(0deg);
                -o-transform: rotate(0deg);
                transform: rotate(0deg);
            }

            .navbar-toggler span:nth-child(3) {
              -webkit-transform: rotate(-45deg);
              -moz-transform: rotate(-45deg);
              -o-transform: rotate(-45deg);
              transform: rotate(-45deg);
            }


            .navbar-toggler.collapsed span:nth-child(1) {
              left: 0;
            }

            .navbar-toggler:not(.collapsed) span:nth-child(1) {
              top: 18px;
              width: 0%;
              left: 50%;
            }


            .navbar-toggler:not(.collapsed) span:nth-child(4) {
              top: 18px;
              width: 0%;
              left: 50%;
            }

            .navbar-toggler.collapsed span:nth-child(4) {
              left: 0;
            }

            @import "card";
            /* @import "accordion"; */
            /* @import "breadcrumb"; */
            @import "pagination";
            /* @import "badge"; */
            /* @import "alert"; */
            /* @import "progress"; */
            /* @import "list-group"; */
            /* @import "close"; */
            /* @import "toasts"; */
            /* @import "modal"; */
            /* @import "tooltip"; */
            @import "popover";
            /* @import "carousel"; */
            /* @import "spinners"; */
            /* @import "offcanvas"; */
            /* @import "placeholders"; */
            @import "helpers";
            @import "utilities/api";

            // Page Transitions
            @view-transition {
              navigation: auto;
            }

            // annimations
            .fade-up{
              opacity: 0;
              transform: translateY(20px);
              transition: opacity .8s ease-out, transform .9s ease-out;
            }

            .fade-up.visible {
              opacity: 1;
              transform: translateY(0);
            }

            .fade-in{
              transition: opacity .8s ease-out, transform .9s ease-out;
              opacity: 0;
            }

            .fade-in.visible {
              opacity: 1;
            }

            body{
                padding-top: 4.5rem;
            }

        """";

    }
}
