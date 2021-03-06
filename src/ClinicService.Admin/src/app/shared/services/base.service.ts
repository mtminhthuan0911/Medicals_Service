import { throwError } from 'rxjs';

export abstract class BaseService {

    constructor() { }

    protected handleError(errorResponse: any) {

        if (errorResponse.error.message) {
            return throwError(errorResponse.error.message || 'Internal server error');
        }

        if (errorResponse.error.errors) {
            const modelStateErrors = [];

            // for now just concatenate the error descriptions, alternative we could simply pass the entire error response upstream
            for (const errorMsg of errorResponse.error.errors) {
                modelStateErrors.push(errorMsg);
            }

            return throwError(modelStateErrors || 'Internal server error');
        }

        return throwError('Internal server error');
    }
}
